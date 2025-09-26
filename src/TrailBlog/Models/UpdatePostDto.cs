using System.ComponentModel.DataAnnotations;

namespace TrailBlog.Api.Models
{
    public sealed class UpdatePostDto
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

    }
}
