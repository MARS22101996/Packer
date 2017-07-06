using Microsoft.AspNetCore.Builder;

namespace TaskManagerUI.Infrastructure.Authorization
{
    public static class AuthorizationExtension
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}
