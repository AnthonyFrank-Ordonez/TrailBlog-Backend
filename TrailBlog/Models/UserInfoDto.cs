using TrailBlog.Entities;

namespace TrailBlog.Models
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
    }
}
