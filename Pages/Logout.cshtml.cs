using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Invalidate the session
                user.SessionId = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }
    }
}