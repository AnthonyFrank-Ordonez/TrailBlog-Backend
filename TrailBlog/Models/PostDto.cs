using System.ComponentModel.DataAnnotations;

namespace TrailBlog.Api.Models
{
    public class PostDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Community Name is required")]
        public string CommunityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Community Id is required")]
        public Guid CommunityId { get; set; } = Guid.Empty;
    }
}
