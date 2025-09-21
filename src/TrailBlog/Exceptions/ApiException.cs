namespace TrailBlog.Api.Exceptions
{
    public class ApiException : Exception
    {
        public string? ResourceType { get; }
        public object? Identifier { get; }
        public int StatusCode { get; }

        // Not Found
        public ApiException(string resourceType, object identifier) 
            : base($"{resourceType} with id '{identifier}' was not found")
        {
            ResourceType = resourceType;
            Identifier = identifier;
            StatusCode = 404;
        }

        // Empty Collection
        public ApiException(string resourceType)
            : base($"No {resourceType.ToLower()} were found")
        {
            ResourceType = resourceType;
            StatusCode = 404;
        }

        // Custom Scenarios with specific status codes
        public ApiException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        // Validation Errors
        public ApiException(string field, string validationMessage)
            : base($"Validation failed for {field}: {validationMessage}")
        {
            ResourceType = field;
            StatusCode = 400;
        }
    }
}
