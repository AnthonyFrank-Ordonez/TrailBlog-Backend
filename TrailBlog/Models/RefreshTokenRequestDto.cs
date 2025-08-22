namespace TrailBlog.Models
{
    public class RefreshTokenRequestDto
    {
        public Guid Id { get; set; }
        public required string RefreshToken { get; set; }
    }
}
