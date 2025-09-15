namespace TrailBlog.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserCommunity> UserCommunities { get; set; } = new List<UserCommunity>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
