using Microsoft.AspNetCore.Identity;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<AuthService> logger,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork, 
        IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger _logger = logger;

        public async Task<AuthResultDto> LoginAsync(LoginDto request)
        {
            var user = await _userRepository.GetUserByUsernameWithRolesAsync(request.Username);

            if (user is null || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                throw new ApplicationException("Invalid Credentials");
            

            if (user.IsRevoked)
                throw new ApplicationException($"User account '{user.Email}' is revoked");
            

            return await CreateAuthResponse(user);
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto request)
        {
            if (await _userRepository.UsernameExistsAsync(request.Username))
                throw new ApplicationException($"Username '{request.Username}' already exists.");
            

            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new ApplicationException($"Email '{request.Email}' already exists.");


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
            var userRole = await _roleRepository.GetRoleByNameAsync("User");

            if (userRole != null)
            {
                var assignedUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    
                };

                await _userRoleRepository.AddAsync(assignedUserRole);
            }

            await _unitOfWork.SaveChangesAsync();

            var userWithRoles = await _userRepository.GetUserByIdWithRolesAsync(user.Id);

            if (userWithRoles == null)
                throw new NotFoundException("User not found after registration.");
            

            return await CreateAuthResponse(userWithRoles);
        }

        public async Task<OperationResultDto> LogoutAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                throw new NotFoundException($"User with the id of {id} not found, failed to logout");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Logout success");

        } 

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);

            if (user is null)
                throw new UnauthorizedException("Invalid or expired refresh token.");
            

            var userWithRoles = await _userRepository.GetUserByIdWithRolesAsync(user.Id);

            if (userWithRoles is null)
                throw new NotFoundException($"User with the id of {user.Id} not found.");
            

            return await CreateAuthResponse(userWithRoles);
        }


        public async Task<OperationResultDto> AssignRoleAsync(AssignRoleDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            var role = await _roleRepository.GetRoleByNameAsync(request.RoleName);

            if (user is null || role is null)
                throw new NotFoundException($"User with id of {request.UserId} or Role {request.RoleName} not found");
            

            var existingUserRole = await _userRoleRepository.RoleExistAsync(request.UserId, role.Id);

            if (existingUserRole != null)
            {
                throw new ApplicationException($"Failed to assigned role. User {user.Username} already has the role");
            }

            var assignedRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            };

            await _userRoleRepository.AddAsync(assignedRole);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Role assign successful");
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            _logger.LogInformation("Send Refresh Token {refreshToken}", refreshToken);
            _logger.LogInformation("User Refresh Token {refreshToken}", user?.RefreshToken);

            if (user is null || user?.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
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

            await _unitOfWork.SaveChangesAsync();

            return refreshToken;
        }   

        private async Task<AuthResultDto> CreateAuthResponse(User user)
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
