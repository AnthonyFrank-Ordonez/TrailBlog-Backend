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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Username { get; set; }

        public string CommunityName { get; set; } = string.Empty;

        public Guid CommunityId { get; set; }   

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public List<CommentResponseDto>? Comments { get; set; }

        public List<PostReactionSummaryDto> Reactions { get; set; } = new List<PostReactionSummaryDto>();

        public List<int> UserReactionsIds { get; set; } = new List<int>();

        public bool IsOwner { get; set; }
        public bool IsSaved { get; set; }

        public int TotalComment { get; set; }

        public int TotalReactions { get; set; }
    }
}
        