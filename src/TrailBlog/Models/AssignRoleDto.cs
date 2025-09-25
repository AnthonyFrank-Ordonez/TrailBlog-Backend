namespace TrailBlog.Api.Models
{
    public sealed class AssignRoleDto
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
