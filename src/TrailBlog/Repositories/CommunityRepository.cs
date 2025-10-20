using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class CommunityRepository : Repository<Community>, ICommunityRepository
    {
        public CommunityRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Community> GetCommunityDetails(bool readOnly = true)
        {
            var query = _dbSet
                .Include(cy => cy.User)
                .Include(cy => cy.Posts);

            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<Community> GetRecentCommunities()
        {
            return GetCommunityDetails()
                .OrderByDescending(cy => cy.CreatedAt);
        }

        public async Task<Community?> GetCommunityDetailsAsync(Guid communityId)
        {
            return await GetCommunityDetails()
                .FirstOrDefaultAsync(c => c.Id == communityId);
        }

        public IQueryable<Community> SearchCommunities(string searchQuery)
        {
            return GetCommunityDetails()
                .Where(c => c.Name.Contains(searchQuery));
        }


    }
}
