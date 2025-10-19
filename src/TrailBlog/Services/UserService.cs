using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger,
        IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUserRepository _userrepository = userRepository;
        private readonly ILogger _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<PagedResultDto<PublicUserResponseDto>> GetUsersPagedAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (page > 100) pageSize = 100;

            var query = _userrepository.GetUserDetails();

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new PublicUserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsRevoked = user.IsRevoked,
                })
                .ToListAsync();

            return new PagedResultDto<PublicUserResponseDto>
            {
                Data = users,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

        }

        public async Task<PagedResultDto<UserResponseDto>> GetUsersWithRolesPagedAsync(int page, int pageSize)
        {

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (page > 100) pageSize = 100;

            var query = _userrepository.GetUserDetails();

            var totalCount = await query.CountAsync();

            var usersWithRoles = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsRevoked = user.IsRevoked,
                    RevokedAt = user.RevokedAt
                })
                .ToListAsync();

            return new PagedResultDto<UserResponseDto>
            {
                Data = usersWithRoles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

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
            var adminUsers = await _userrepository
                .GetAdminUsers()
                .Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsRevoked = user.IsRevoked,
                    RevokedAt = user.RevokedAt
                })
                .ToListAsync();

            return adminUsers;
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

        public async Task<OperationResultDto> DeleteUserAsync(Guid userId)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            _logger.LogInformation("Deleteing user with the id: {UserId}...", userId);

            await _userrepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Successfuly deleted user");
        }
    }
}
