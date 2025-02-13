using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Helpers;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Pages
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthDbContext _dbContext;
        private readonly EmailSender _emailSender;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, AuthDbContext dbContext, EmailSender emailSender)
        {
            this.userManager = userManager;
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public void OnGet(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null) return NotFound();

            // ===== Enforce Minimum Password Age Policy =====
            // Example: Do not allow a password change if the password was changed less than 1 day ago.
            var minPasswordAge = TimeSpan.FromDays(1);
            if (user.PasswordChangedDate.HasValue && DateTime.UtcNow - user.PasswordChangedDate.Value < minPasswordAge)
            {
                ModelState.AddModelError("", "Your password was changed too recently. Please try again later.");
                return Page();
            }

            // ===== Enforce Password History Check =====
            // Retrieve all historical records for this user.
            var passwordHistories = await _dbContext.PasswordHistories
                .Where(ph => ph.UserId == user.Id)
                .ToListAsync();

            int matchingCount = 0;
            foreach (var ph in passwordHistories)
            {
                // Compare the new password with each historical hash.
                var verificationResult = userManager.PasswordHasher.VerifyHashedPassword(user, ph.PasswordHash, NewPassword);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    matchingCount++;
                }
            }

            // If the same password has been used two (or more) times already, do not allow it.
            if (matchingCount >= 2)
            {
                ModelState.AddModelError("", "You have already used this password the maximum allowed number of times. Please choose a different password.");
                return Page();
            }

            var result = await userManager.ResetPasswordAsync(user, Token, NewPassword);
            if (result.Succeeded)
            {
                // Update password history
                _dbContext.PasswordHistories.Add(new PasswordHistory
                {
                    UserId = user.Id,
                    PasswordHash = userManager.PasswordHasher.HashPassword(user, NewPassword),
                    DateChanged = DateTime.UtcNow
                });

                user.PasswordChangedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("Login");
            }

            ModelState.AddModelError("", "Failed to reset password.");
            return Page();
        }
    }
}