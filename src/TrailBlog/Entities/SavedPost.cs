namespace TrailBlog.Api.Entities
{
    public class SavedPost
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime SavedAt { get; set; }
    }
}
