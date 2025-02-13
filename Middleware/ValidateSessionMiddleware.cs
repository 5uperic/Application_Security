using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Middleware
{
    public class ValidateSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user != null)
            {
                var sessionId = context.Session.GetString("SessionId");
                if (sessionId != user.SessionId)
                {
                    // Invalidate the session
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Login");
                    return;
                }
            }

            await _next(context);
        }
    }

}
