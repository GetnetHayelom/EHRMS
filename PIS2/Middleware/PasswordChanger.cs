using Microsoft.AspNetCore.Identity;
using PIS2.Models;

namespace PIS2.Middleware
{
    public class PasswordChanger
    {
        private readonly RequestDelegate _next;

        public PasswordChanger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<userModel> userManager)
        {
            // User is not authenticated.
            // Let the normal authentication/authorization pipeline handle it.
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            // Current request path
            var path = context.Request.Path;

            // Allow the password change page itself.
            // Otherwise we would create an infinite redirect.
            if (path.StartsWithSegments("/Account/ChangePassword"))
            {
                await _next(context);
                return;
            }

            // Allow logout.
            if (path.StartsWithSegments("/Account/Logout"))
            {
                await _next(context);
                return;
            }

            // Get currently logged-in Identity user
            var user = await userManager.GetUserAsync(context.User);

            if (user == null)
            {
                await _next(context);
                return;
            }

            // User must change password
            if (user.MustChangePassword)
            {
                context.Response.Redirect("/Account/ChangePassword");
                return;
            }

            await _next(context);
        }
    }
}

