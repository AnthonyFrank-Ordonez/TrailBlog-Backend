using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Models
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserResponseDto? User { get; set; }

    }
}
    