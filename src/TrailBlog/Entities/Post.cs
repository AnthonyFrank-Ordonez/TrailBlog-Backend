using System.ComponentModel.DataAnnotations.Schema;

namespace TrailBlog.Api.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Author { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid CommunityId { get; set; }
        public Community Community { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [NotMapped]
        public int TotalLikes => Likes?.Count ?? 0;
        [NotMapped]
        public int TotalComments => Comments?.Count ?? 0;
    }
}