using Microsoft.EntityFrameworkCore;
using TrailBlog.Data;
using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDto?>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsRevoked = u.IsRevoked,
                    RevokedAt = u.RevokedAt
                })
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetUserAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsRevoked = u.IsRevoked,
                    RevokedAt = u.RevokedAt
                })
                .FirstOrDefaultAsync();

            if (user is null)
                return null;

            return user;
        }

        public async Task<UserResponseDto?> RevokedUserAsync(Guid Id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (user is null)
                return null;

            user.IsRevoked = true;
            user.RevokedAt = DateTime.UtcNow;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsRevoked = user.IsRevoked,
                RevokedAt = user.RevokedAt
            };

            return userResponse;
        }


    }
}
