using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Test.Models;
namespace test.Components.Pages;

public partial class QuizEngine
{
    [Parameter]
    public int QuizId { get; set; }

    private Quiz? quiz;
    private List<Question> questions = new();
    private Dictionary<Question, List<Answer>> questionAnswers = new();
    private Dictionary<int, int> selectedAnswers = new();
    private bool loading = true;
    private bool quizSubmitted = false;
    private float score = 0;
    private float totalPossibleScore = 0;
    private int? currentStudentId;

    // New properties for timed questions
    private int currentQuestionIndex = 0;
    private int timeLeft = 0;
    private System.Threading.Timer? timer;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentStudentId();

        // Check if the student has already reached max attempts before loading the quiz
        if (await HasReachedMaxAttempts())
        {
            // Redirect to quiz list with an error message
            Navigation.NavigateTo("/quiz-list?error=maxattempts");
            return;
        }

        await LoadQuiz();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && questions.Count > 0 && !quizSubmitted)
        {
            StartTimerForCurrentQuestion();
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    private async Task GetCurrentStudentId()
    {
        try
        {
            // Get the current user
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext!.User);

            if (user != null)
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
                    // Handle case where there's no matching student in the custom table
                    Logger.LogError("No matching student found in Students table");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting current student ID: {Message}", ex.Message);
        }
    }

    private async Task LoadQuiz()
    {
        try
        {
            // Fetch the quiz with its questions and answers
            quiz = await DbContext.Quizzes.FindAsync(QuizId);
            if (quiz == null)
            {
                loading = false;
                return;
            }

            // Load questions for this quiz
            questions = await DbContext.Questions
                .Where(q => q.QuizID == QuizId)
                .ToListAsync();

            // Load answers for each question
            foreach (var question in questions)
            {
                var answers = await DbContext.Answers
                    .Where(a => a.QuestionID == question.QuestionID)
                    .ToListAsync();

                questionAnswers[question] = answers;
                totalPossibleScore += question.Score ?? 1;
            }

            // Initialize timer for the first question
            if (questions.Count > 0)
            {
                timeLeft = GetQuestionTimeLimit(questions[0]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading quiz: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private void SelectAnswer(int questionId, int answerId)
    {
        selectedAnswers[questionId] = answerId;
    }

    private int GetQuestionTimeLimit(Question question)
    {
        return question.QuestionDifficulty switch
        {
            Question.Difficulty.easy => 20,
            Question.Difficulty.medium => 40,
            Question.Difficulty.hard => 60,
            _ => 40  // Default fallback
        };
    }

    private void StartTimerForCurrentQuestion()
    {
        // Dispose of any existing timer
        timer?.Dispose();

        if (currentQuestionIndex < questions.Count)
        {
            var question = questions[currentQuestionIndex];
            timeLeft = GetQuestionTimeLimit(question);

            // Create new timer that ticks every second
            timer = new System.Threading.Timer(_ =>
            {
                timeLeft--;

                // When time runs out, move to next question
                if (timeLeft <= 0)
                {
                    InvokeAsync(() =>
                    {
                        NextQuestion();
                        StateHasChanged();
                    });
                }
                else
                {
                    InvokeAsync(StateHasChanged);
                }
            },
            null,
            0,
            1000); // Update every second
        }
    }

    private void NextQuestion()
    {
        // Stop the current timer
        timer?.Dispose();

        // Move to the next question if available
        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            StartTimerForCurrentQuestion();
        }
        else
        {
            // We've reached the end of the questions
            currentQuestionIndex = questions.Count; // Move to the submit screen

            // Auto-submit the quiz after the last question times out
            InvokeAsync(async () =>
            {
                await SubmitQuiz();
                StateHasChanged();
            });
        }
    }

    private async Task SubmitQuiz()
    {
        // Dispose of any active timer
        timer?.Dispose();

        // Calculate score
        foreach (var question in questions)
        {
            if (selectedAnswers.TryGetValue(question.QuestionID, out int answerId))
            {
                var answer = questionAnswers[question].FirstOrDefault(a => a.AnswerID == answerId);
                if (answer != null && answer.IsCorrect)
                {
                    score += question.Score ?? 1;
                }
            }
        }

        // Create quiz attempt record in database
        try
        {
            if (currentStudentId == null)
            {
                await GetCurrentStudentId();

                if (currentStudentId == null)
                {
                    // Still no student ID, use a fallback approach or show an error
                    Logger.LogError("Cannot save quiz attempt: No student ID found");
                    return;
                }
            }

            var attempt = new QuizAttempt
            {
                QuizID = QuizId,
                StudentID = currentStudentId.Value,
                TotalScore = score,
                IsCompleted = true
            };

            DbContext.QuizAttempts.Add(attempt);
            await DbContext.SaveChangesAsync();
            Logger.LogInformation("Quiz attempt saved for student ID: {StudentId}", currentStudentId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving quiz attempt: {Message}", ex.Message);
        }

        quizSubmitted = true;
    }

    private async Task<bool> HasReachedMaxAttempts()
    {
        if (!currentStudentId.HasValue) return false;

        try
        {
            // Get number of attempts for this quiz by this student
            int attemptCount = await DbContext.QuizAttempts
                .Where(a => a.QuizID == QuizId && a.StudentID == currentStudentId.Value)
                .CountAsync();

            // Check if max attempts (2) has been reached
            return attemptCount >= 2;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking quiz attempts: {Message}", ex.Message);
            return false; // On error, allow the attempt to proceed
        }
    }
}
