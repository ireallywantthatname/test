using Microsoft.AspNetCore.Components;
namespace test.Components.Pages;

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Test.Models;

public partial class Register
{

    [Parameter]
    public string? Role { get; set; }

    private Student student = new();
    private Educator educator = new();
    private string studentConfirmPassword = string.Empty;
    private string educatorConfirmPassword = string.Empty;
    private bool passwordError = false;
    private bool educatorPasswordError = false;

    private List<string> errors = new List<string>();

    protected override void OnInitialized()
    {
        student ??= new();
        educator ??= new();
    }

    protected override void OnParametersSet()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("role", out var roleValue))
        {
            Role = roleValue.ToString().ToLower();
        }
        else
        {
            Role = null;
        }
    }

    private async Task HandleStudentRegistration()
    {
        errors.Clear();
        passwordError = false;

        // Check if passwords match
        if (student.Password != studentConfirmPassword)
        {
            passwordError = true;
            return;
        }

        try
        {
            Logger.LogInformation("Student registration attempt for username: {Username}", student?.Username);

            var identityUser = new IdentityUser { UserName = student!.Username, Email = student.Username };
            var result = await UserManager.CreateAsync(identityUser, student.Password);

            if (result.Succeeded)
            {
                // Add a claim to identify this user as a Student
                var userTypeClaim = new Claim("UserType", "Student");
                await UserManager.AddClaimAsync(identityUser, userTypeClaim);
                
                // Save the user to our custom database
                await DbContext.Students.AddAsync(student);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation("Student registration successful for username: {Username}", student.Username);
                
                // Don't sign in here - instead redirect to login page
                Navigation.NavigateTo("/login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Student registration failed for username: {Username}, Error: {Error}", student.Username, error.Description);
                    errors.Add(error.Description);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while registering student with username: {Username}", student?.Username);
            errors.Add("An unexpected error occurred. Please try again later.");
        }
    }

    private async Task HandleEducatorRegistration()
    {
        errors.Clear();
        educatorPasswordError = false;

        // Check if passwords match
        if (educator.Password != educatorConfirmPassword)
        {
            educatorPasswordError = true;
            return;
        }

        try
        {
            Logger.LogInformation("Educator registration attempt for username: {Username}", educator?.Username);

            var identityUser = new IdentityUser { UserName = educator!.Username, Email = educator.Username };
            var result = await UserManager.CreateAsync(identityUser, educator.Password);

            if (result.Succeeded)
            {
                // Add a claim to identify this user as an Educator
                var userTypeClaim = new Claim("UserType", "Educator");
                await UserManager.AddClaimAsync(identityUser, userTypeClaim);
                
                // Save the user to our custom database
                await DbContext.Educators.AddAsync(educator);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation("Educator registration successful for username: {Username}", educator.Username);
                
                // Don't sign in here - instead redirect to login page
                Navigation.NavigateTo("/login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Educator registration failed for username: {Username}, Error: {Error}", educator.Username, error.Description);
                    errors.Add(error.Description);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while registering educator with username: {Username}", educator?.Username);
            errors.Add("An unexpected error occurred. Please try again later.");
        }
    }
}