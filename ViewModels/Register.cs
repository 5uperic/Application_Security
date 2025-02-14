using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplication1.ViewModels
{
    public class Register : IValidatableObject
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Credit Card Number must be 16 digits.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid Credit Card Number.")]
        public string? CreditCardNumber { get; set; }

        [Required]
        public string? Gender { get; set; }  // "Male", "Female", "Other"

        [Required]
        public string? DeliveryAddress { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile? Photo { get; set; }  // This will hold the uploaded photo file

        [Required]
        public string? AboutMe { get; set; }  // No validation for special characters

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 12, ErrorMessage = "The password must be at least 12 characters long.")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var emailRegex = new Regex(@"^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$", RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(Email))
            {
                yield return new ValidationResult("Invalid email format", new[] { nameof(Email) });
            }
        }
    }
}
