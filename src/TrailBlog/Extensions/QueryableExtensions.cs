using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResultDto<TDto>> ToRandomPagedAsync<TDto, TEntity>(
            this IQueryable<TEntity> query,
            IMemoryCache cache,
            int page,
            int pageSize,
            string? sessionId,
            Func<TEntity, TDto> selector,
            IHttpContextAccessor httpContextAccessor) where TEntity : class
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            var cacheKey = $"random_query_{sessionId}";

            if (!cache.TryGetValue(cacheKey, out List<object>? shuffledIds) || shuffledIds == null)
            {
                // Get primary keys
                var allIds = await query
                    .Select(e => EF.Property<object>(e, "Id"))
                    .ToListAsync();

                // Shuffle the IDs
                shuffledIds = allIds
                    .OrderBy(x => Guid.NewGuid())
                    .ToList();

                // Cache for 10 minutes
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                cache.Set(cacheKey, shuffledIds, cacheOptions);
            }

            var totalCount = shuffledIds.Count;

            var pagedIds = shuffledIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var entities = await query
                .Where(e => pagedIds.Contains(EF.Property<object>(e, "Id")))
                .ToListAsync();

            // Convert to DTO and maintain order
            var idProperty = typeof(TEntity).GetProperty("Id");
            var entityDict = entities.ToDictionary(e => idProperty!.GetValue(e)!);


            var orderedResults = pagedIds
                .Where(id => entityDict.ContainsKey(id))
                .Select(id => selector(entityDict[id]))
                .ToList();

            httpContextAccessor.HttpContext?.Response?.Headers.Append("X-Session-Id", sessionId);

            return new PagedResultDto<TDto>
            {
                Data = orderedResults,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}
