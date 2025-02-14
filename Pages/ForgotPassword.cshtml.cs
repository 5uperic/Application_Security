using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Helpers;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Email { get; set; }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, EmailSender emailSender)
        {
            this.userManager = userManager;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null && await userManager.IsEmailConfirmedAsync(user))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Page("/ResetPassword", null, new { token, email = user.Email }, Request.Scheme);
                Console.WriteLine(resetLink);

                await _emailSender.SendEmailAsync(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");
                Console.WriteLine($"Password reset link sent to {user.Email}");
            }
    
            return RedirectToPage("ForgotPasswordConfirmation"); // Ensure this matches the page name
        }
    }
}