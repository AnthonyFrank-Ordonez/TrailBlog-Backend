namespace TrailBlog.Api.Models
{
    public sealed class CommentResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }   
}
