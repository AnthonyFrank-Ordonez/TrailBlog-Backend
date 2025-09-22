namespace TrailBlog.Api.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message) : base(message, 401) { }
    }
}
    