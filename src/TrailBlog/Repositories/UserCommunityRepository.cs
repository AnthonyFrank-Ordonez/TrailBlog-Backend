using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class UserCommunityRepository : Repository<UserCommunity>, IUserCommunityRepository
    {
        public UserCommunityRepository(ApplicationDbContext context) : base(context) { }


        public async Task<IEnumerable<UserCommunity>> GetUserCommunitiesAsync(Guid userId)
        {
            return await _dbSet
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Community)
                .Include(uc => uc.Community)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserCommunity>> GetCommunityMemberAsync(Guid communityId)
        {
            return await _dbSet
                .Where(uc => uc.CommunityId == communityId)
                .Include(uc => uc.User)
                .ToListAsync();
        }

        public async Task<UserCommunity?> ExistingMemberAsync(Guid communityId, Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(uc => uc.Community.Id == communityId && uc.UserId == userId);
        }



    }
}
