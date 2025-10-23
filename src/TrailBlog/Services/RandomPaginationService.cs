using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public class RandomPaginationService : IRandomPaginationService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RandomPaginationService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<PagedResultDto<T>> GetRandomPagedAsync<T, TEntity>(
            IQueryable<TEntity> query, 
            int page, 
            int pageSize, 
            string sessionId, 
            Func<TEntity, T> selector) where TEntity : class
        {
            // Validate paramters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            var cacheKey = $"random_query_{sessionId}";

            if (!_cache.TryGetValue(cacheKey, out List<object>? shuffledIds) || shuffledIds == null)
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

                _cache.Set(cacheKey, shuffledIds, cacheOptions);
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

            _httpContextAccessor.HttpContext?.Response?.Headers.Append("X-Session-Id", sessionId);

            return new PagedResultDto<T>
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
