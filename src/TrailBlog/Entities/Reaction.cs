namespace TrailBlog.Api.Entities
{
    public class Reaction
    {
        public Guid Id { get; set; }

        public bool IsLike { get; set; } = false;
        public bool IsDislike { get; set; } = false;
        public DateTime ReactedAt { get; set; } 

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
    }
}
