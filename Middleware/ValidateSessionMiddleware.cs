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
            Console.WriteLine("ValidateSessionMiddleware: Invoked");

            // Check if the user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("ValidateSessionMiddleware: User is authenticated");

                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    Console.WriteLine("ValidateSessionMiddleware: User found in database");

                    var sessionId = context.Session.GetString("SessionId");
                    Console.WriteLine($"ValidateSessionMiddleware: Session ID in session: {sessionId}");
                    Console.WriteLine($"ValidateSessionMiddleware: Session ID in database: {user.SessionId}");

                    if (sessionId != user.SessionId)
                    {
                        Console.WriteLine("ValidateSessionMiddleware: Session ID mismatch. Invalidating session.");
                        await signInManager.SignOutAsync();
                        context.Session.Clear(); // Clear the session data
                        context.Response.Redirect("/Login");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("ValidateSessionMiddleware: User not found in database");
                }
            }
            else
            {
                Console.WriteLine("ValidateSessionMiddleware: User is not authenticated");
            }

            await _next(context);
        }
    }

}
