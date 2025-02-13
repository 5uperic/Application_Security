using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Middleware;
using WebApplication1.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Configure Database Context
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Lockout period of 5 minutes
    options.Lockout.MaxFailedAccessAttempts = 3;                      // Lockout after 3 failed attempts
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Register EmailSender
builder.Services.AddTransient<EmailSender>();

// Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Prevent client-side script access to the cookie
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";

    // Security Enhancements
    options.Cookie.HttpOnly = true; // Prevents client-side access to cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures cookies are sent only over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Helps prevent Cross-Site Request Forgery (CSRF)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Reduces cookie expiration time
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Add custom middleware to validate sessions
app.UseMiddleware<ValidateSessionMiddleware>();

app.MapRazorPages();

app.Run();