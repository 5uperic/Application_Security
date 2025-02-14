using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication1.Model;
using System.Web;
using System.ComponentModel.DataAnnotations;  // Add this if using HttpUtility (Install System.Web.HttpUtility if necessary)

namespace WebApplication1.Pages
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        [Required]
        public string Message { get; set; }

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (userId == null || token == null)
            {
                Message = "Invalid email confirmation request.";
                return Page();
            }

            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Received Token: {token}");


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Message = $"Unable to load user with ID '{userId}'.";
                return Page();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Console.WriteLine("Email confirmed successfully.");
                return RedirectToPage("/EmailConfirmed");
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Confirmation error: {error.Description}");
            }

            Message = "Email confirmation failed. Please try again or contact support.";
            return Page();
        }
    }
}
