using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class UserCommunityRepository : Repository<UserCommunity>, IUserCommunityRepository
    {
        public UserCommunityRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<UserCommunity> GetUserCommunitiesDetails(bool readOnly = true)
        {
            var query = _dbSet
                .Include(uc => uc.User)
                .Include(uc => uc.Community)
                    .ThenInclude(c => c.User);

            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<UserCommunity> GetUserCommunitiesAsync(Guid userId)
        {
            return GetUserCommunitiesDetails()
                .Where(uc => uc.UserId == userId);
        }

        public IQueryable<UserCommunity> GetCommunityMembersAsync(Guid communityId)
        {
            return GetUserCommunitiesDetails()
                .Where(uc => uc.CommunityId == communityId);
        }

        public async Task<UserCommunity?> ExistingMemberAsync(Guid communityId, Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(uc => uc.Community.Id == communityId && uc.UserId == userId);
        }



    }
}
