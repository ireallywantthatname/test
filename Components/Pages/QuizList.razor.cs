using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Test.Models;
namespace test.Components.Pages;

public partial class QuizList
{
    private List<Quiz> quizzes = new();
    private Dictionary<int, int> questionCounts = new();
    private Dictionary<int, int> quizAttemptCounts = new();
    private Dictionary<int, int> totalQuizAttempts = new();
    private bool loading = true;
    private int? currentStudentId;
    private string? errorMessage;
    private string? currentUserName;

    private bool isEducator => !currentStudentId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentStudentId();
        await LoadQuizzes();
    }

    protected override void OnInitialized()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("error", out var errorParam))
        {
            if (errorParam == "maxattempts")
            {
                errorMessage = "You have already reached the maximum number of attempts allowed for this quiz.";
            }
        }
    }

    private async Task GetCurrentStudentId()
    {
        try {
            // Get the current user
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext!.User);
            
            if (user != null)
            {
                // Store the username for educator filtering
                currentUserName = user.UserName;
                
                // Find the corresponding Student entity using the username
                var student = await DbContext.Students.FirstOrDefaultAsync(s => s.Username == user.UserName);
                
                if (student != null)
                {
                    currentStudentId = student.StudentId;
                    Logger.LogInformation("Found student ID: {StudentId}", currentStudentId);
                }
                else
                {
                    // Handle case where there's no matching student in the custom table
                    Logger.LogError("Possible Educator; No matching student found in Students table");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting current student ID: {Message}", ex.Message);
        }
    }

    private async Task LoadQuizzes()
    {
        try
        {
            // Load quizzes based on user role
            if (isEducator)
            {
                // For educators, first get the educator ID based on username
                var educator = await DbContext.Educators.FirstOrDefaultAsync(e => e.Username == currentUserName);
                
                if (educator != null)
                {
                    // Only show quizzes created by this educator
                    quizzes = await DbContext.Quizzes
                        .Where(q => q.EducatorID == educator.EducatorID)
                        .ToListAsync();
                    
                    Logger.LogInformation("Loaded {Count} quizzes created by educator {Username} with ID {EducatorId}", 
                        quizzes.Count, currentUserName, educator.EducatorID);
                    
                    // For each quiz, count the total number of attempts by all students
                    foreach (var quiz in quizzes)
                    {
                        var attemptCount = await DbContext.QuizAttempts
                            .Where(a => a.QuizID == quiz.QuizId)
                            .CountAsync();
                        
                        totalQuizAttempts[quiz.QuizId] = attemptCount;
                        Logger.LogInformation("Quiz {QuizId} has {TotalAttemptCount} total attempts", quiz.QuizId, attemptCount);
                    }
                }
                else
                {
                    Logger.LogWarning("No educator record found for username {Username}", currentUserName);
                    quizzes = new List<Quiz>(); // Empty list if no educator found
                }
            }
            else
            {
                // For students, show all quizzes
                quizzes = await DbContext.Quizzes.ToListAsync();
            }

            // Count questions for each quiz
            foreach (var quiz in quizzes)
            {
                var count = await DbContext.Questions
                    .Where(q => q.QuizID == quiz.QuizId)
                    .CountAsync();

                questionCounts[quiz.QuizId] = count;

                if (currentStudentId.HasValue)
                {
                    // Count attempts by the current student for this quiz
                    var attemptCount = await DbContext.QuizAttempts
                        .Where(a => a.QuizID == quiz.QuizId && a.StudentID == currentStudentId.Value)
                        .CountAsync();

                    quizAttemptCounts[quiz.QuizId] = attemptCount;
                    Logger.LogInformation("Quiz {QuizId} has {AttemptCount} attempts by current student", quiz.QuizId, attemptCount);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading quizzes: {Message}", ex.Message);
        }
        finally
        {
            loading = false;
        }
    }

    private void Start(int quizId)
    {
        if (quizAttemptCounts.TryGetValue(quizId, out int attempts) && attempts >= 2)
        {
            return; // Don't navigate if max attempts reached
        }
        
        Navigation.NavigateTo($"/quiz/{quizId}");
    }
}