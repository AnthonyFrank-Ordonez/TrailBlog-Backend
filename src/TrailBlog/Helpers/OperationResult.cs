using TrailBlog.Api.Models;

namespace TrailBlog.Api.Helpers
{
    public class OperationResult
    {
        public static OperationResultDto Success(string message) => 
            new() { Success = true, Message = message };

        public static OperationResultDto Failure(string message) =>
            new() { Success = false, Message = message };
    }
}
