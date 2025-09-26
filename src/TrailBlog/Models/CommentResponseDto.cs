using System.Text.Json.Serialization;

namespace TrailBlog.Api.Models
{
    public sealed class CommentResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid UserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid PostId { get; set; }
    }   
}
