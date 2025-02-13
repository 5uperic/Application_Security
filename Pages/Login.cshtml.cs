// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using WebApplication1.Model;
// using WebApplication1.ViewModels;
// using WebApplication1.Helpers;
// using Microsoft.EntityFrameworkCore;


// namespace WebApplication1.Pages
// {
//     public class LoginModel : PageModel
//     {
//         [BindProperty]
//         public Login LModel { get; set; }

//         [BindProperty]
//         public string NewPassword { get; set; }


//         private readonly SignInManager<ApplicationUser> signInManager;
//         private readonly UserManager<ApplicationUser> userManager;
//         private readonly AuthDbContext _dbContext;
//         private readonly EmailSender _emailSender;
//         public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext dbContext, EmailSender emailSender)
//         {
//             this.signInManager = signInManager;
//             this.userManager = userManager;
//             _dbContext = dbContext;
//             _emailSender = emailSender;
//         }

//         public void OnGet()
//         {

//         }

//         public async Task<IActionResult> OnPostDontLogoutAsync()
//         {
//             return RedirectToPage("Index");
//         }


//         public async Task<IActionResult> OnPostLogoutAsync()
//         {
//             await signInManager.SignOutAsync();
//             Console.WriteLine("User logged out.");
//             return RedirectToPage("Login");
//         }

//         public async Task<IActionResult> OnPostForgotPasswordAsync(string email)
//         {
//             var user = await userManager.FindByEmailAsync(email);
//             if (user != null && await userManager.IsEmailConfirmedAsync(user))
//             {
//                 var token = await userManager.GeneratePasswordResetTokenAsync(user);
//                 var resetLink = Url.Page("/ResetPassword", null, new { token, email = user.Email }, Request.Scheme);

//                 await _emailSender.SendEmailAsync(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");
//                 Console.WriteLine($"Password reset link sent to {user.Email}");
//             }

//             return RedirectToPage("ForgotPasswordConfirmation");
//         }


//         public async Task<IActionResult> OnPostResetPasswordAsync(string token, string email, string newPassword)
//         {
//             var user = await userManager.FindByEmailAsync(email);
//             if (user == null) return NotFound();

//             var result = await userManager.ResetPasswordAsync(user, token, newPassword);
//             if (result.Succeeded)
//             {
//                 // Update password history
//                 _dbContext.PasswordHistories.Add(new PasswordHistory
//                 {
//                     UserId = user.Id,
//                     PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword),
//                     DateChanged = DateTime.UtcNow
//                 });

//                 user.PasswordChangedDate = DateTime.UtcNow;
//                 await _dbContext.SaveChangesAsync();

//                 return RedirectToPage("Login");
//             }

//             ModelState.AddModelError("", "Failed to reset password.");
//             return Page();
//         }



//         public async Task<IActionResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
//         {
//             var user = await userManager.FindByIdAsync(userId);
//             if (user == null) return NotFound();

//             var passwordHistory = await _dbContext.PasswordHistories
//                 .Where(ph => ph.UserId == userId)
//                 .OrderByDescending(ph => ph.DateChanged)
//                 .Take(2)
//                 .ToListAsync();

//             foreach (var history in passwordHistory)
//             {
//                 if (userManager.PasswordHasher.VerifyHashedPassword(user, history.PasswordHash, newPassword) == PasswordVerificationResult.Success)
//                 {
//                     ModelState.AddModelError("", "You cannot reuse your last 2 passwords.");
//                     return Page();
//                 }
//             }

//             var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
//             if (result.Succeeded)
//             {
//                 _dbContext.PasswordHistories.Add(new PasswordHistory
//                 {
//                     UserId = userId,
//                     PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword),
//                     DateChanged = DateTime.UtcNow
//                 });

//                 user.PasswordChangedDate = DateTime.UtcNow;
//                 await _dbContext.SaveChangesAsync();

//                 return RedirectToPage("Profile");
//             }

//             return Page();
//         }



//         public async Task<IActionResult> OnPostChangePasswordAsync()
//         {
//             if (ModelState.IsValid)
//             {
//                 var userId = User.Identity.Name;
//                 var currentPassword = LModel.Password;  // Assuming you're using the password from LModel
//                 var result = await ChangePasswordAsync(userId, currentPassword, NewPassword);

//                 if (result is RedirectToPageResult)
//                 {
//                     // Success message
//                     return RedirectToPage("Profile");
//                 }
//                 else
//                 {
//                     // Add error messages from ChangePasswordAsync
//                 }
//             }

//             return Page();
//         }





//         public async Task<IActionResult> OnPostAsync()
//         {
//             if (ModelState.IsValid)
//             {

//                 var user = await userManager.GetUserAsync(User);
//                 if (DateTime.UtcNow - user.PasswordChangedDate < TimeSpan.FromDays(1))
//                 {
//                     ModelState.AddModelError("", "You cannot change your password more than once a day.");
//                     return Page();
//                 }

//                 if (DateTime.UtcNow - user.PasswordChangedDate > TimeSpan.FromDays(90))
//                 {
//                     ModelState.AddModelError("", "Your password has expired. Please change it.");
//                     return RedirectToPage("ChangePassword");
//                 }

//                 var result = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

//                 if (result.Succeeded)
//                 {
//                     Console.WriteLine($"User {LModel.Email} successfully logged in at {DateTime.UtcNow}");
//                     return RedirectToPage("Index");
//                 }
//                 else if (result.IsLockedOut)
//                 {
//                     Console.WriteLine($"User {LModel.Email} is locked out at {DateTime.UtcNow}");
//                     ModelState.AddModelError("", "Your account is locked. Please try again 5 minutes later.");
//                 }
//                 else if (result.IsNotAllowed)
//                 {
//                     Console.WriteLine($"User {LModel.Email} attempted login without email confirmation at {DateTime.UtcNow}");
//                     ModelState.AddModelError("", "You must confirm your email before logging in.");
//                 }
//                 else
//                 {
//                     Console.WriteLine($"Invalid login attempt for user {LModel.Email} at {DateTime.UtcNow}");
//                     ModelState.AddModelError("", "Invalid login attempt.");
//                 }
//             }

//             return Page();
//         }

//     }
// }

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public string RecaptchaSiteKey { get; private set; }


        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            _userManager = userManager;
            this.configuration = configuration;
            RecaptchaSiteKey = configuration["RecaptchaSettings:SiteKey"];
            this.httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }


        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            Console.WriteLine("User logged out.");
            return RedirectToPage("Login");
        }

        private async Task<bool> ValidateRecaptcha(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("reCAPTCHA token is null or empty");
                return false;
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsync(
                    "https://www.google.com/recaptcha/api/siteverify",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "secret", configuration["RecaptchaSettings:SecretKey"] },
                        { "response", token }
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();
                    Console.WriteLine($"reCAPTCHA validation result - Success: {jsonResponse.Success}, Score: {jsonResponse.Score}");
                    return jsonResponse.Success && jsonResponse.Score >= 0.5;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"reCAPTCHA API error response: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating reCAPTCHA: {ex.Message}");
                return false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                // Extract reCAPTCHA token from the form
                var recaptchaToken = Request.Form["g-recaptcha-response"].ToString();
                Console.WriteLine($"Received reCAPTCHA token (length: {recaptchaToken?.Length ?? 0})");

                if (string.IsNullOrEmpty(recaptchaToken))
                {
                    ModelState.AddModelError("", "reCAPTCHA verification failed. Please try again.");
                    return Page();
                }

                var isRecaptchaValid = await ValidateRecaptcha(recaptchaToken);
                Console.WriteLine($"reCAPTCHA validation result: {isRecaptchaValid}");

                if (!isRecaptchaValid)
                {
                    ModelState.AddModelError("", "Invalid reCAPTCHA verification. Please try again.");
                    return Page();
                }

                var user = await _userManager.FindByEmailAsync(LModel.Email);
                if (user != null)
                {
                    // Invalidate previous session
                    user.SessionId = Guid.NewGuid().ToString(); // Generate a new session ID
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Store the new session ID in the current session
                    HttpContext.Session.SetString("SessionId", user.SessionId);
                    Console.WriteLine($"New Session ID: {user.SessionId}");

                    if (user.PasswordChangedDate.HasValue && DateTime.UtcNow - user.PasswordChangedDate.Value > TimeSpan.FromDays(90))
                    {
                        // Redirect the user to a “Force Change Password” page.
                        return RedirectToPage("ForceChangePassword");
                    }
                }


                var result = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    Console.WriteLine($"User {LModel.Email} successfully logged in at {DateTime.UtcNow}");
                    return RedirectToPage("Index");
                }
                else if (result.IsLockedOut)
                {
                    Console.WriteLine($"User {LModel.Email} is locked out at {DateTime.UtcNow}");
                    ModelState.AddModelError("", "Your account is locked. Please try again 5 minutes later.");
                }
                else if (result.IsNotAllowed)
                {
                    Console.WriteLine($"User {LModel.Email} attempted login without email confirmation at {DateTime.UtcNow}");
                    ModelState.AddModelError("", "You must confirm your email before logging in.");
                }
                else
                {
                    Console.WriteLine($"Invalid login attempt for user {LModel.Email} at {DateTime.UtcNow}");
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return Page();
        }
    }

}
