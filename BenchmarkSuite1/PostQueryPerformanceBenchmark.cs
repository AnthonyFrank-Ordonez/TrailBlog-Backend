using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Models;
using Microsoft.VSDiagnostics;

namespace TrailBlog.Api.Benchmarks
{
    [SimpleJob(warmupCount: 3, targetCount: 5)]
    [CPUUsageDiagnoser]
    public class PostQueryPerformanceBenchmark
    {
        private ApplicationDbContext _context = null !;
        private Guid _testUserId;
        private List<Post> _testPosts = null !;
        [GlobalSetup]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite("Data Source=:memory:").Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            // Create test data
            _testUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            };
            var community = new Community
            {
                Id = communityId,
                Name = "Test Community",
                UserId = userId,
                User = user
            };
            var userCommunity = new UserCommunity
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                CommunityId = communityId,
                JoinedDate = DateTime.UtcNow
            };
            _context.Users.Add(user);
            _context.Communities.Add(community);
            _context.UserCommunities.Add(userCommunity);
            // Create 50 posts in the same community
            _testPosts = new List<Post>();
            for (int i = 0; i < 50; i++)
            {
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = $"Test Post {i}",
                    Content = $"Content {i}",
                    Author = "testuser",
                    Slug = $"test-post-{i}",
                    UserId = userId,
                    User = user,
                    CommunityId = communityId,
                    Community = community
                };
                _testPosts.Add(post);
                _context.Posts.Add(post);
            }

            _context.SaveChanges();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        [Benchmark(Description = "Query without UserCommunities eager load")]
        public async Task<List<PostResponseDto>> QueryWithoutUserCommunitiesAsync()
        {
            var userId = _testUserId;
            var result = await _context.Posts.Where(p => p.Status == PostStatus.Published).Include(p => p.Community).Include(p => p.Reactions).Include(p => p.SavedPosts).Include(p => p.Comments).AsNoTracking().Select(p => new PostResponseDto { Id = p.Id, Title = p.Title, Content = p.Content, Author = p.Author, Slug = p.Slug, CreatedAt = p.CreatedAt, CommunityName = p.Community.Name, CommunityId = p.CommunityId, IsOwner = userId.HasValue && p.UserId == userId, IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId), TotalComment = p.Comments.Count(c => !c.IsDeleted), TotalReactions = p.Reactions.Count, }).Take(10).ToListAsync();
            return result;
        }

        [Benchmark(Description = "Query WITH UserCommunities eager load")]
        public async Task<List<PostResponseDto>> QueryWithUserCommunitiesAsync()
        {
            var userId = _testUserId;
            var result = await _context.Posts.Where(p => p.Status == PostStatus.Published).Include(p => p.Community).ThenInclude(c => c.UserCommunities).Include(p => p.Reactions).Include(p => p.SavedPosts).Include(p => p.Comments).AsNoTracking().Select(p => new PostResponseDto { Id = p.Id, Title = p.Title, Content = p.Content, Author = p.Author, Slug = p.Slug, CreatedAt = p.CreatedAt, CommunityName = p.Community.Name, CommunityId = p.CommunityId, IsOwner = userId.HasValue && p.UserId == userId, IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId), TotalComment = p.Comments.Count(c => !c.IsDeleted), TotalReactions = p.Reactions.Count, }).Take(10).ToListAsync();
            return result;
        }

        [Benchmark(Description = "Query WITH UserCommunities and IsUserJoined property")]
        public async Task<List<PostResponseDtoWithUserJoined>> QueryWithIsUserJoinedAsync()
        {
            var userId = _testUserId;
            var result = await _context.Posts.Where(p => p.Status == PostStatus.Published).Include(p => p.Community).ThenInclude(c => c.UserCommunities).Include(p => p.Reactions).Include(p => p.SavedPosts).Include(p => p.Comments).AsNoTracking().Select(p => new PostResponseDtoWithUserJoined { Id = p.Id, Title = p.Title, Content = p.Content, Author = p.Author, Slug = p.Slug, CreatedAt = p.CreatedAt, CommunityName = p.Community.Name, CommunityId = p.CommunityId, IsOwner = userId.HasValue && p.UserId == userId, IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId), IsUserJoined = userId.HasValue && p.Community.UserCommunities.Any(uc => uc.UserId == userId), TotalComment = p.Comments.Count(c => !c.IsDeleted), TotalReactions = p.Reactions.Count, }).Take(10).ToListAsync();
            return result;
        }
    }

    public class PostResponseDtoWithUserJoined : PostResponseDto
    {
        public bool IsUserJoined { get; set; }
    }
}