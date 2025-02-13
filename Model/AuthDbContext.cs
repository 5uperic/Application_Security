using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("AuthConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'AuthConnectionString' is not found or is null.");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure PasswordHistory relationship
            builder.Entity<PasswordHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PasswordHistories)
                .HasForeignKey(ph => ph.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data or additional configuration if needed
        }

        public DbSet<PasswordHistory> PasswordHistories { get; set; }
    }
}
