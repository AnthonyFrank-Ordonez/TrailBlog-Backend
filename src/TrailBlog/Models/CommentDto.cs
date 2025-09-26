using System.ComponentModel.DataAnnotations;

namespace TrailBlog.Api.Models
{
    public sealed class CommentDto
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "PostId is required")]
        public Guid PostId { get; set; }
    }
}
