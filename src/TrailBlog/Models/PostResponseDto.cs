using System.Text.Json.Serialization;

namespace TrailBlog.Api.Models
{
    public sealed class PostResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; } = string.Empty;

        public string CommunityName { get; set; } = string.Empty;

        public Guid CommunityId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();

        public int TotalLike { get; set; }

        public int TotalComment { get; set; }

        public bool IsLiked { get; set; }

        public bool IsDisliked { get; set; }
    }
}
        