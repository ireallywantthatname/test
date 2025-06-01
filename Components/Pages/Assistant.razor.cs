using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace test.Components.Pages;

public partial class Assistant
{
    private List<QuizFeedback> feedbacks = new();
    public bool isEducator = false;
    private bool loading = true;
    private int? currentStudentId;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentStudentId();

        if (currentStudentId.HasValue)
        {
            await LoadFeedbackHistory();
        }
        else
        {
            isEducator = true;
        }

        loading = false;
    }

    private async Task GetCurrentStudentId()
    {
        try
        {
            // Get the current user
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext!.User);

            if (user != null)
            {
                // Option 1: Check user type by claim (preferred)
                var claims = await UserManager.GetClaimsAsync(user);
                var userTypeClaim = claims.FirstOrDefault(c => c.Type == "UserType");
                
                isEducator = userTypeClaim?.Value == "Educator";
                
                if (!isEducator) // Is a student
                {
                    // Find the corresponding Student entity using the username
                    var student = await DbContext.Students.FirstOrDefaultAsync(s => s.Username == user.UserName);

                    if (student != null)
                    {
                        currentStudentId = student.StudentId;
                        Logger.LogInformation("Current student ID: {StudentId}", currentStudentId);
                    }
                    else
                    {
                        Logger.LogError("No matching student found in Students table");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting current student ID: {Message}", ex.Message);
        }
    }

    private async Task LoadFeedbackHistory()
    {
        try
        {
            if (!currentStudentId.HasValue)
            {
                Logger.LogWarning("Cannot load feedback: No student ID found");
                return;
            }

            // Get all quiz attempts for the student
            var attempts = await DbContext.QuizAttempts
                .Where(a => a.StudentID == currentStudentId.Value && a.IsCompleted)
                .Select(a => a.AttemptID)
                .ToListAsync();

            if (attempts.Any())
            {
                // Get feedback for each attempt
                feedbacks = await DbContext.QuizFeedbacks
                    .Where(f => attempts.Contains(f.AttemptID))
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                Logger.LogInformation("Loaded {Count} feedback entries for student ID: {StudentId}",
                    feedbacks.Count, currentStudentId);
            }
            else
            {
                Logger.LogInformation("No quiz attempts found for student ID: {StudentId}", currentStudentId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading feedback history: {Message}", ex.Message);
        }
    }
}