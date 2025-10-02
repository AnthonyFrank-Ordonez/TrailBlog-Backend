using System.Text.Json.Serialization;

namespace TrailBlog.Api.Models
{
    public sealed class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsRevoked { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? RevokedAt { get; set; }

    }
}
