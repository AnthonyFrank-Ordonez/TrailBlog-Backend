namespace TrailBlog.Api.Models
{
    public sealed class OperationResultDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
