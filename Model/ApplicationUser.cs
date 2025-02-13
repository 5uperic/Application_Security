using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string DeliveryAddress { get; set; }
        public string AboutMe { get; set; }
        public string CreditCardNumber { get; set; }
        public string PhotoPath { get; set; }  // This will store the file path for the uploaded photo

        public DateTime? PasswordChangedDate { get; set; } = DateTime.UtcNow;

        public string? SessionId { get; set; } // Track the current session ID
        public DateTime? LastLogin { get; set; } // Track the last login time

        // Navigation property for password history
        public ICollection<PasswordHistory> PasswordHistories { get; set; }
    }
}
