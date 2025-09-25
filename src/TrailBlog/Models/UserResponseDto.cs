namespace TrailBlog.Api.Models
{
    public sealed class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

    }
}
