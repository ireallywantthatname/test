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
builder.Services.AddDbContextFactory<TestDbContext>(options => options.UseSqlite(connectionString));
// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for testing
    options.Lockout.MaxFailedAccessAttempts = 10;    // Increase to prevent lockout during testing
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Adjust lockout time
})
    .AddEntityFrameworkStores<TestDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
