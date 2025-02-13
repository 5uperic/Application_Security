using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthDbContext _dbContext;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, AuthDbContext dbContext)
        {
            this.userManager = userManager;
            _dbContext = dbContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var passwordHistory = await _dbContext.PasswordHistories
                .Where(ph => ph.UserId == user.Id)
                .OrderByDescending(ph => ph.DateChanged)
                .Take(2)
                .ToListAsync();

            foreach (var history in passwordHistory)
            {
                if (userManager.PasswordHasher.VerifyHashedPassword(user, history.PasswordHash, NewPassword) == PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "You cannot reuse your last 2 passwords.");
                    return Page();
                }
            }

            var result = await userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            if (result.Succeeded)
            {
                _dbContext.PasswordHistories.Add(new PasswordHistory
                {
                    UserId = user.Id,
                    PasswordHash = userManager.PasswordHasher.HashPassword(user, NewPassword),
                    DateChanged = DateTime.UtcNow
                });

                user.PasswordChangedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}