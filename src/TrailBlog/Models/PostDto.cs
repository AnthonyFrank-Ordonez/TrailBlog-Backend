using System.ComponentModel.DataAnnotations;

namespace TrailBlog.Api.Models
{
    public sealed class PostDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Community Id is required")]
        public Guid CommunityId { get; set; } = Guid.Empty;
    }
}