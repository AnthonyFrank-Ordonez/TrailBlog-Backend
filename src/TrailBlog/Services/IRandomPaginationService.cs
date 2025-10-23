using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IRandomPaginationService
    {
        Task<PagedResultDto<T>> GetRandomPagedAsync<T, TEntity>(
            IQueryable<TEntity> query,
            int page,
            int pageSize,
            string sessionId,
            Func<TEntity, T> selector) where TEntity : class;
    }
}
