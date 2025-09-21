using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class AuthService(ApplicationDbContext context, IUserRepository userRepository, IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AuthResultDto> LoginAsync(LoginDto request)
        {
            var user = await _userRepository.GetUserByUsernameWithRolesAsync(request.Username);

            if (user is null || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return AuthResult.Failure("Invalid Credentials");
            }

            if (user.IsRevoked)
            {
                return AuthResult.Failure("User account is revoked");
            }

            return await CreateAuthResponse(true, user);
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto request)
        {
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                return AuthResult.Failure("Username already exists.");
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return AuthResult.Failure("Email already exists.");
            }

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(new User(), request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);

            // Assign default user role
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole != null)
            {
                var assignedUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    
                };

                _context.UserRoles.Add(assignedUserRole);
            }

            await _unitOfWork.SaveChangesAsync();

            var userWithRoles = await _userRepository.GetUserByIdWithRolesAsync(user.Id);

            if (userWithRoles == null)
            {
                return AuthResult.Failure("User not found after registration.");
            }

            return await CreateAuthResponse(true, userWithRoles);
        }

        public async Task<bool> LogoutAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync();

            return true;

        } 

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);

            if (user is null)
            {
                return AuthResult.Failure("Invalid or expired refresh token.");
            }

            var userWithRoles = await _userRepository.GetUserByIdWithRolesAsync(user.Id);

            if (userWithRoles is null)
            {
                return AuthResult.Failure("User not found.");
            }

            return await CreateAuthResponse(true, userWithRoles);
        }


        public async Task<bool> AssignRoleAsync(AssignRoleDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName);

            if (user is null || role is null)
            {
                return false;
            }

            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == role.Id);

            if (existingUserRole != null)
            {
                return true; // Already has the role
            }

            var assignedRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(assignedRole);

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = AuthTokenHelper.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<AuthResultDto> CreateAuthResponse(bool status, User user)
        {
            var accessToken = AuthTokenHelper.GenerateAccessToken(user, _configuration);
            var refreshToken = await GenerateAndSaveRefreshToken(user);
            var userResponseDto = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return AuthResult.Success(accessToken, refreshToken, userResponseDto);
        }


    }
}
