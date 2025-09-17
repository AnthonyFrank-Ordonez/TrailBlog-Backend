using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrailBlog.Data;
using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostResponseDto?>> GetPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    Username = p.User.Username,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                })
                .ToListAsync();
        }

        public async Task<PostResponseDto?> GetPostAsync(Guid id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Community)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    Username = p.User.Username,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return null;

            return post;
        }

        public async Task<PostResponseDto?> CreatePostAsync(PostDto post, Guid userId)
        {
            if (post is null)
            {
                return null;
            }

            var newPost = new Post();

            newPost.Title = post.Title;
            newPost.Content = post.Content;
            newPost.Author = post.Author;
            newPost.CreatedAt = DateTime.UtcNow;
            newPost.UpdatedAt = DateTime.UtcNow;
            newPost.Slug = post.Title.ToLower().Replace(" ", "-");
            newPost.UserId = userId;
            newPost.CommunityId = post.CommunityId;

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);


            return new PostResponseDto
            {
                Id = newPost.Id,
                Title = newPost.Title,
                Content = newPost.Content,
                Author = newPost.Author,
                Slug = newPost.Slug,
                CreatedAt = newPost.CreatedAt,
                Username = user?.Username ?? string.Empty,
                CommunityName = post.CommunityName,
                CommunityId = newPost.CommunityId,
            };
        }

        public async Task<Post?> UpdatePostAsync(Guid id, PostDto post, Guid userId)
        {
            var existingPost = await _context.Posts.FindAsync(id);

            if (existingPost is null || existingPost.UserId != userId)
            {
                return null;
            }

            existingPost.Title = string.IsNullOrEmpty(post.Title)
                ? existingPost.Title
                : post.Title;

            existingPost.Content = string.IsNullOrEmpty(post.Content)
                ? existingPost.Content
                : post.Content;

            existingPost.Author = string.IsNullOrEmpty(post.Author)
                ? existingPost.Author
                : post.Author;

            existingPost.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(post.Title))
                existingPost.Slug = post.Title.ToLower().Replace(" ", "-");

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();

            return existingPost;
        }

        public async Task<Post?> DeletePostAsync(Guid id, Guid userId, bool IsAdmin = false)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return null;
            }

            if (post.UserId != userId && !IsAdmin)
            {
                return null; // User is not authorized to delete this post
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<List<CommunityResponseDto>> GetAllCommunityBlogsAsync()  
        {
            var communityBlogs = await _context.Communities
                .Include(c => c.Posts.OrderByDescending(p => p.CreatedAt).Take(5))
                .Select(c => new CommunityResponseDto
                {
                    CommunityName = c.Name,
                    Id = c.Id,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt,
                    }).ToList()
                })
                .ToListAsync();

            return communityBlogs;

        }

        public async Task<List<PostResponseDto>> GetRecentPostsAsync(int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    Username = p.User.Username,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                })
                .ToListAsync();

            return posts;
        }
    }

}
