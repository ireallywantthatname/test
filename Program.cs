using test.Components;
using Test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TestDB");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure DbContext with SQLite
builder.Services.AddDbContext<TestDbContext>(options => options.UseSqlite(connectionString));

// Register factory with correct lifetime (scoped instead of singleton)
builder.Services.AddDbContextFactory<TestDbContext>(options => options.UseSqlite(connectionString),
    lifetime: ServiceLifetime.Scoped); // Change from default singleton to scoped

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for testing
    options.Lockout.MaxFailedAccessAttempts = 10;   // Increase to prevent lockout during testing
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Adjust lockout time
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<TestDbContext>();

// Add authentication services
builder.Services.AddAuthentication(options => 
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});

// Add authorization services and define policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EducatorOnly", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            // Check for the "UserType" claim with value "Educator"
            return user.Identity?.IsAuthenticated == true && 
                  user.HasClaim(c => c.Type == "UserType" && c.Value == "Educator");
        }));
});

// Configure cookie policy for authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/access-denied";
    
    // This will still redirect unauthenticated users to the login page
    options.LoginPath = "/login";

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Add these middleware components for Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map Identity endpoints (login, register, etc.)
app.MapIdentityApi<IdentityUser>();

app.Run();
