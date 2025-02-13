using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using WebApplication1.Helpers;
using WebApplication1.Model;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly EmailSender emailSender;

        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public Register RModel { get; set; }

        public string RecaptchaSiteKey { get; private set; }

        public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        EmailSender emailSender,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.RecaptchaSiteKey = configuration["RecaptchaSettings:SiteKey"];

            Console.WriteLine($"RegisterModel initialized with RecaptchaSiteKey: {RecaptchaSiteKey}");
        }

        private async Task<bool> ValidateRecaptcha(string token)
        {
            Console.WriteLine("Starting reCAPTCHA validation");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("reCAPTCHA token is null or empty");
                return false;
            }

            try
            {
                Console.WriteLine("Sending request to reCAPTCHA API");
                Console.WriteLine($"Using secret key: {configuration["RecaptchaSettings:SecretKey"]?.Substring(0, 5)}... (truncated)");

                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsync(
                    "https://www.google.com/recaptcha/api/siteverify",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                    { "secret", configuration["RecaptchaSettings:SecretKey"] },
                    { "response", token }
                    }));

                Console.WriteLine($"reCAPTCHA API response status: {response.StatusCode}");

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
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public void OnGet()
        {
        }

        public bool ShowConfirmationModal { get; set; } = false;

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                Console.WriteLine("Model state is valid, proceeding with registration");
                
                var recaptchaToken = Request.Form["g-recaptcha-response"].ToString();
                Console.WriteLine($"Received reCAPTCHA token (length: {recaptchaToken?.Length ?? 0})");

                if (string.IsNullOrEmpty(recaptchaToken))
                {
                    Console.WriteLine("No reCAPTCHA token found in form submission");
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                    return Page();
                }

                var isRecaptchaValid = await ValidateRecaptcha(recaptchaToken);
                Console.WriteLine($"reCAPTCHA validation result: {isRecaptchaValid}");

                if (!isRecaptchaValid)
                {
                    Console.WriteLine("reCAPTCHA validation failed");
                    ModelState.AddModelError(string.Empty, "Invalid reCAPTCHA verification. Please try again.");
                    return Page();
                }
                

                if (RModel.Password.Length < 12)
                {
                    ModelState.AddModelError(nameof(RModel.Password), "The password must be at least 12 characters long.");
                    return Page();
                }

                var existingUser = await userManager.FindByEmailAsync(RModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered.");
                    return Page();
                }

                string photoPath = null;

                if (RModel.Photo != null)
                {
                    if (Path.GetExtension(RModel.Photo.FileName).ToLower() != ".jpg")
                    {
                        ModelState.AddModelError("Photo", "Only JPG images are allowed.");
                        return Page();
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos");
                    Directory.CreateDirectory(uploadsFolder);

                    photoPath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}.jpg");
                    using (var fileStream = new FileStream(photoPath, FileMode.Create))
                    {
                        await RModel.Photo.CopyToAsync(fileStream);
                    }
                }

                string encryptedCreditCard = EncryptionHelper.Encrypt(RModel.CreditCardNumber);

                var user = new ApplicationUser
                {
                    FullName = RModel.FullName,
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    PhoneNumber = RModel.PhoneNumber,
                    CreditCardNumber = encryptedCreditCard,
                    Gender = RModel.Gender,
                    DeliveryAddress = RModel.DeliveryAddress,
                    AboutMe = RModel.AboutMe,
                    PhotoPath = photoPath
                };

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", RModel.FullName));

                    // Generate email confirmation token
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Do NOT manually encode the token here
                    var confirmationLink = Url.Page("/ConfirmEmail", null, new { userId = user.Id, token = token }, Request.Scheme);

                    Console.WriteLine($"User ID: {user.Id}");
                    Console.WriteLine($"Token: {token}");
                    Console.WriteLine($"Generated confirmation link: {confirmationLink}");

                    // Send confirmation email using SendGrid
                    var emailSender = new EmailSender();
                    var emailContent = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.";
                    Console.WriteLine($"Email content: {emailContent}");

                    await emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");

                    Console.WriteLine($"Confirmation email sent to {user.Email}. Link: {confirmationLink}");

                    // Show the confirmation modal
                    ShowConfirmationModal = true;
                    return Page();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return Page();
        }
    }

    public class RecaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; }
        public string Action { get; set; }
        public DateTime ChallengeTs { get; set; }
        public string HostName { get; set; }
    }

}


