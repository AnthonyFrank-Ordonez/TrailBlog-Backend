using System.Text.Json.Serialization;

namespace TrailBlog.Api.Models
{
    public sealed class CommunityResponseDto
    {
        public Guid Id { get; set; }
        public string CommunityName { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        public string Owner { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PostResponseDto>? Posts { get; set; }
    }
}
        