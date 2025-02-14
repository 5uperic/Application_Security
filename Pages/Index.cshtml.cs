using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Helpers;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // This property will hold the current user's details
        [Required]
        public ApplicationUser CurrentUser { get; set; }

         // Properties to hold both encrypted and decrypted credit card values
         [Required]
        public string EncryptedCreditCard { get; set; }
        [Required]
        public string DecryptedCreditCard { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the currently logged-in user using the Identity UserManager
            CurrentUser = await _userManager.GetUserAsync(User);

            EncryptedCreditCard = CurrentUser.CreditCardNumber;
                // Decrypt the credit card information
            DecryptedCreditCard = EncryptionHelper.Decrypt(CurrentUser.CreditCardNumber);

            if (CurrentUser == null)
            {
                // If for some reason the user is not found, redirect to the login page (or handle accordingly)
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }
    }
}
