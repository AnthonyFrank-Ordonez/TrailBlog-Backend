using System.ComponentModel.DataAnnotations;

namespace TrailBlog.Api.Models
{
    public class UpdateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
