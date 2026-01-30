namespace TrailBlog.Api.Entities
{
    public class Community
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CommunitySlug { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<UserCommunity> UserCommunities { get; set; } = new List<UserCommunity>();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}