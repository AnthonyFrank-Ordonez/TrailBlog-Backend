using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Exceptions;

namespace TrailBlog.Api.Services
{
    public class UserService(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUserRepository _userrepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<IEnumerable<PublicUserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userrepository.GetAllAsync();

            return users.Select(u => new PublicUserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsRevoked = u.IsRevoked,
            }).ToList();

        }

        public async Task<PublicUserResponseDto?> GetUserAsync(Guid userId)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null) 
                throw new NotFoundException($"No user found with the id of {userId}");

            return new PublicUserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsRevoked = user.IsRevoked,
            };
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersWithRolesAsync()
        {

            var users = await _userrepository.GetAllUsersWithRolesAsync();

            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsRevoked = u.IsRevoked,
                RevokedAt = u.RevokedAt
            }).ToList();

        }

        public async Task<UserResponseDto?> GetUserWithRolesAsync(Guid userId)
        {
            var user = await _userrepository.GetUserByIdWithRolesAsync(userId);

            if (user is null)
                throw new NotFoundException($"No user found with the id of {userId}");

            return new UserResponseDto
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
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAdminUsersAsync()
        {
            var adminUsers = await _userrepository.GetAllAdminUsersAsync();

            return adminUsers.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsRevoked = u.IsRevoked,
                RevokedAt = u.RevokedAt
            }).ToList();
        }

        public async Task<OperationResultDto> RevokedUserAsync(Guid userId)
        {
            var user = await _userrepository.GetUserByIdWithRolesAsync(userId);

            if (user is null)
                throw new NotFoundException($"No user found with the id of {userId}");

            user.IsRevoked = true;
            user.RevokedAt = DateTime.UtcNow;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;

            var revokedUser = _userrepository.UpdateAsync(userId, user);
            await _unitOfWork.SaveChangesAsync();

            if (revokedUser is null)
                throw new ApiException("An error occured. Fauled to revoke user");

            return OperationResult.Success("Successfully revoked user");
                
        }
    }
}
