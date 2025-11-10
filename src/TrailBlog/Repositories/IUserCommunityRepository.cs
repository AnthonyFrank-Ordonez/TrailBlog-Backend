using System.Linq.Expressions;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IUserCommunityRepository : IRepository<UserCommunity>
    {
        IQueryable<UserCommunity> GetUserCommunitiesDetails(bool readOnly = true);
        IQueryable<UserCommunity> GetUserCommunitiesAsync(Guid userId);
        Task<UserCommunity?> GetUserCommunityAsync(Expression<Func<UserCommunity, bool>> predicate, bool isReadOnly = true);
        IQueryable<UserCommunity> GetCommunityMembersAsync(Guid communityId);
        Task<UserCommunity?> ExistingMemberAsync(Guid communityId, Guid userid);
        Task<UserCommunity?> UpdateAsync(Guid userId, Guid communityId, UserCommunity entity);
    }
}
