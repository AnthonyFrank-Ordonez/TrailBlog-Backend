namespace TrailBlog.Api.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public String Content { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

    }
}
