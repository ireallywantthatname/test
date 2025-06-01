using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
namespace test.Components.Pages;

public partial class Login
{
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    public string? Error { get; set; }

    private class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    [SupplyParameterFromForm]
    private LoginViewModel loginModel { get; set; } = new LoginViewModel();


    private List<string> errors = new();
    private bool shouldRedirect = false;
    private string redirectUrl = string.Empty;

    protected override Task OnInitializedAsync()
    {
        // Check if user is already authenticated using HttpContextAccessor
        var isAuthenticated = HttpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            // User is already logged in, redirect to home
            Logger.LogInformation("Already authenticated user attempted to access login page");
            Navigation.NavigateTo("/");
            return Task.CompletedTask;
        }

        // Handle error messages from query parameters
        if (!string.IsNullOrEmpty(Error))
        {
            switch (Error)
            {
                case "invalid":
                    errors.Add("Invalid username or password.");
                    break;
                case "locked":
                    errors.Add("Your account is locked out. Please try again later.");
                    break;
                case "notallowed":
                    errors.Add("Login is not allowed. Please confirm your email or contact support.");
                    break;
                case "notfound":
                    errors.Add("User not found. Please check your credentials.");
                    break;
                case "unexpected":
                    errors.Add("An unexpected error occurred. Please try again later.");
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private async Task HandleLogin()
    {
        errors.Clear();

        try
        {
            Logger.LogInformation("User login attempt for username: {Username}", loginModel.Username);

            var result = await SignInManager.PasswordSignInAsync(
                loginModel.Username,
                loginModel.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                Logger.LogInformation("User login successful for username: {Username}", loginModel.Username);

                // Get user and check claims to determine redirect
                var user = await UserManager.FindByNameAsync(loginModel.Username);

                if (user != null)
                {
                    var claims = await UserManager.GetClaimsAsync(user);
                    var userTypeClaim = claims.FirstOrDefault(c => c.Type == "UserType");

                    if (userTypeClaim?.Value == "Educator")
                    {
                        redirectUrl = "/quiz-create";
                    }
                    else
                    {
                        // Default to home page for students or unknown user types
                        redirectUrl = "/";
                    }

                    // Use return URL if specified
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        redirectUrl = ReturnUrl;
                    }

                    shouldRedirect = true;
                }
            }
            else
            {
                // Handle login failure
                Logger.LogWarning("Login failed for username: {Username}", loginModel.Username);

                if (result.IsLockedOut)
                {
                    errors.Add("Your account is locked out. Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    errors.Add("Login is not allowed. Please confirm your email or contact support.");
                }
                else
                {
                    errors.Add("Invalid username or password.");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred during login for username: {Username}", loginModel.Username);
            errors.Add("An unexpected error occurred. Please try again later.");
        }
    }
}