namespace TrailBlog.Api.Entities
{
    public class RecentViewedPost
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime ViewedAt { get; set; }

    }
}