using TrailBlog.Api.Models;

namespace TrailBlog.Api.Helpers
{
    public static class AuthResult
    {
        public static AuthResultDto Success(string accessToken, string refreshToken, UserResponseDto user)
        {
            return new AuthResultDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };
        }

        public static AuthResultDto Failure(string message)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = message
            };
        }
    }
}
