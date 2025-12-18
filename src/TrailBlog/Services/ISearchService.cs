using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface ISearchService
    {
        Task<UnifiedSearchResultDto> UnifiedSearchAsync(string query);
    }
}
