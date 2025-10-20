using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IUserCommunityRepository : IRepository<UserCommunity>
    {
        IQueryable<UserCommunity> GetUserCommunitiesDetails(bool readOnly = true);
        IQueryable<UserCommunity> GetUserCommunitiesAsync(Guid userId);
        IQueryable<UserCommunity> GetCommunityMembersAsync(Guid communityId);
        Task<UserCommunity?> ExistingMemberAsync(Guid communityId, Guid userid);
    }
}
