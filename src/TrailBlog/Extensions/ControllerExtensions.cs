using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TrailBlog.Api.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid? GetCurrentUserId(this ControllerBase controller)
        {
            var userIdString = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        public static Guid GetRequiredUserId(this ControllerBase controller)
        {
            return controller.GetCurrentUserId() 
                ?? throw new InvalidOperationException("User is not authenticated.");
        }
    }
}
