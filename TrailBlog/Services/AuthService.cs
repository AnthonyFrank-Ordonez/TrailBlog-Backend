using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TrailBlog.Data;
using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user is null || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid Credentials"
                };
            }

            if (user.IsRevoked)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User account is revoked."
                };
            }

            return await CreateAuthResponse(true, user);
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Username already exists."
                };
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);

            // Assign default user role
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole != null)
            {
                var assignedUserRole = new UserRole();

                assignedUserRole.UserId = user.Id;
                assignedUserRole.RoleId = userRole.Id;
                assignedUserRole.AssignedAt = DateTime.UtcNow;

                _context.UserRoles.Add(assignedUserRole);
            }

            await _context.SaveChangesAsync();

            var userWithRoles = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithRoles == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User not found after registration."
                };
            }

            return await CreateAuthResponse(true, userWithRoles);
        }

        public async Task<bool> LogoutAsync(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);

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
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid or expired refresh token."
                };
            }

            var userWithRoles = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithRoles is null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            return await CreateAuthResponse(true, userWithRoles);
        }


        public async Task<bool> AssignRoleAsync(AssignRoleDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
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

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private async Task<AuthResultDto> CreateAuthResponse(bool status, User user)
        {
            return new AuthResultDto
            {
                Success = status,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user),
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                },
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Secret")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:ExpiryInDays")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
