using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace test.Components.Pages;

public partial class Dashboard
{
    private bool loading = true;
    private string? userType;
    private string? username;
    private int? currentUserId;

    // For educators
    private Quiz? lastCreatedQuiz;
    private int totalQuizzesCreated = 0;
    private int totalQuestionsCreated = 0;

    // For students
    private List<QuizAttempt> recentAttempts = new();
    private float averageScore = 0;
    private int totalAttempts = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserInfo();
        await LoadDashboardData();
        loading = false;
    }

    private async Task LoadUserInfo()
    {
        try
        {
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext!.User);
            if (user != null)
            {
                username = user.UserName;

                // Get user type from claims
                var claims = await UserManager.GetClaimsAsync(user);
                var userTypeClaim = claims.FirstOrDefault(c => c.Type == "UserType");
                userType = userTypeClaim?.Value;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading user info");
        }
    }

    private async Task LoadDashboardData()
    {
        if (userType == "Educator")
        {
            await LoadEducatorData();
        }
        else if (userType == "Student")
        {
            await LoadStudentData();
        }
    }

    private async Task LoadEducatorData()
    {
        try
        {
            var educator = await DbContext.Educators
                .FirstOrDefaultAsync(e => e.Username == username);

            if (educator != null)
            {
                currentUserId = educator.EducatorID;

                // Get total quizzes created
                totalQuizzesCreated = await DbContext.Quizzes
                    .Where(q => q.EducatorID == educator.EducatorID)
                    .CountAsync();

                // Get last created quiz
                lastCreatedQuiz = await DbContext.Quizzes
                    .Where(q => q.EducatorID == educator.EducatorID)
                    .OrderByDescending(q => q.QuizId)
                    .FirstOrDefaultAsync();

                // Get total questions created across all quizzes
                if (lastCreatedQuiz != null)
                {
                    totalQuestionsCreated = await DbContext.Questions
                        .Where(q => DbContext.Quizzes
                            .Where(quiz => quiz.EducatorID == educator.EducatorID)
                            .Select(quiz => quiz.QuizId)
                            .Contains(q.QuizID))
                        .CountAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading educator data");
        }
    }

    private async Task LoadStudentData()
    {
        try
        {
            var student = await DbContext.Students
                .FirstOrDefaultAsync(s => s.Username == username);

            if (student != null)
            {
                currentUserId = student.StudentId;

                // Get recent quiz attempts
                recentAttempts = await DbContext.QuizAttempts
                    .Where(qa => qa.StudentID == student.StudentId)
                    .Include(qa => qa.Quiz)
                    .OrderByDescending(qa => qa.AttemptID)
                    .Take(2)
                    .ToListAsync();

                totalAttempts = recentAttempts.Count;

                // Calculate average score
                if (recentAttempts.Any())
                {
                    averageScore = recentAttempts.Average(qa => qa.TotalScore);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading student data");
        }
    }

    private void NavigateToQuizCreate()
    {
        Navigation.NavigateTo("/quiz-create");
    }

    private void NavigateToQuizList()
    {
        if (userType == "Educator")
        {
            Navigation.NavigateTo("/quiz-list");
        }
        else
        {
            Navigation.NavigateTo("/quiz-list");
        }
    }
}