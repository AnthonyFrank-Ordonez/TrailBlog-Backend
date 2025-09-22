using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IUserCommunityRepository : IRepository<UserCommunity>
    {
        Task<IEnumerable<UserCommunity>> GetUserCommunitiesAsync(Guid userId);
        Task<IEnumerable<UserCommunity>> GetCommunityMemberAsync(Guid communityId);
        Task<UserCommunity?> ExistingMemberAsync(Guid communityId, Guid userid);
    }
}
