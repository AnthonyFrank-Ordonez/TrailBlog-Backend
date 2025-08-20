using TrailBlog.Entities;

namespace TrailBlog.Models
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserInfoDto? User { get; set; }

    }
}
    