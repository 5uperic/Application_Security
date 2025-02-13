using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Model
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateChanged { get; set; }

        // Navigation property to link with ApplicationUser
        public ApplicationUser User { get; set; }
    }

}
