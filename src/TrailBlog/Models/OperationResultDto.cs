namespace TrailBlog.Api.Models
{
    public class OperationResultDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
