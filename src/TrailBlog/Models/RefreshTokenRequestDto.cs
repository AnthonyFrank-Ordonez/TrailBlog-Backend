namespace TrailBlog.Api.Models
{
    public class RefreshTokenRequestDto
    {
        public Guid Id { get; set; }
        public required string RefreshToken { get; set; }
    }
}
