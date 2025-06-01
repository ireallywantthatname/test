using Microsoft.EntityFrameworkCore;
using Test.Models;
namespace test.Components.Pages;

public partial class QuizCreate
{
    private Quiz paper = new();
    private List<Question> questions = new();
    private Dictionary<Question, List<Answer>> questionAnswers = new();
    private int? currentEducatorId;
    private List<string> errors = new();
    private List<string> summaryErrors = new();
    private bool showValidationErrors = false;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentEducatorId();
    }

    private async Task GetCurrentEducatorId()
    {
        try
        {
            // Get the current user
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext!.User);

            if (user != null)
            {
                // Find the corresponding Educator entity using the username
                var educator = await DbContext.Educators.FirstOrDefaultAsync(e => e.Username == user.UserName);

                if (educator != null)
                {
                    currentEducatorId = educator.EducatorID;
                }
                else
                {
                    // Handle case where there's no matching educator in the custom table
                    Logger.LogError("No matching educator found in Educators table");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting current educator ID: {Message}", ex.Message);
        }
    }

    private bool ValidateQuiz()
    {
        errors.Clear();
        summaryErrors.Clear();

        // These errors are handled inline, so we collect them but they won't be in the summary
        if (string.IsNullOrWhiteSpace(paper.QuizName))
        {
            errors.Add("Quiz title is required");
        }

        if (string.IsNullOrWhiteSpace(paper.QuizDescription))
        {
            errors.Add("Quiz description is required");
        }

        // This is a general error that should be in the summary
        if (questions.Count == 0)
        {
            errors.Add("At least one question is required");
            summaryErrors.Add("At least one question is required");
            return errors.Count == 0;
        }

        // For each question, we track if there's a general issue
        bool hasQuestionError = false;
        bool hasAnswerError = false;
        bool hasCorrectAnswerError = false;

        foreach (var question in questions)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionBody))
            {
                errors.Add("All questions must have text");
                hasQuestionError = true;
            }

            var answers = questionAnswers[question];
            if (!answers.Any())
            {
                errors.Add("Each question must have at least one answer");
                summaryErrors.Add("Each question must have at least one answer");
            }
            else
            {
                if (answers.Any(a => string.IsNullOrWhiteSpace(a.Body)))
                {
                    errors.Add("All answers must have text");
                    hasAnswerError = true;
                }

                if (!answers.Any(a => a.IsCorrect))
                {
                    errors.Add("Each question must have at least one correct answer");
                    hasCorrectAnswerError = true;
                }
            }
        }

        // Add summary messages for general issues
        if (hasQuestionError)
        {
            summaryErrors.Add("Some questions are missing text");
        }

        if (hasAnswerError)
        {
            summaryErrors.Add("Some answers are missing text");
        }

        if (hasCorrectAnswerError)
        {
            summaryErrors.Add("Some questions don't have a correct answer selected");
        }

        return errors.Count == 0;
    }

    private async Task ValidateAndSave()
    {
        showValidationErrors = true;

        if (ValidateQuiz())
        {
            await SavePaper();
            showValidationErrors = false;
        }
    }

    private async Task SavePaper()
    {
        try
        {
            if (currentEducatorId == null)
            {
                await GetCurrentEducatorId();

                if (currentEducatorId == null)
                {
                    // Still no educator ID, cannot save
                    Logger.LogError("Cannot save quiz: No educator ID found");
                    summaryErrors.Add("Cannot save quiz: No educator ID found");
                    return;
                }
            }

            // Set the educator ID before saving
            paper.EducatorID = currentEducatorId.Value;

            DbContext.Quizzes.Add(paper);
            await DbContext.SaveChangesAsync();

            foreach (var question in questions)
            {
                question.QuizID = paper.QuizId;
                DbContext.Questions.Add(question);
                await DbContext.SaveChangesAsync();

                foreach (var answer in questionAnswers[question])
                {
                    answer.QuestionID = question.QuestionID;
                    DbContext.Answers.Add(answer);
                }
            }

            await DbContext.SaveChangesAsync();

            // Clear the form after successful save
            paper = new();
            questions.Clear();
            questionAnswers.Clear();
            errors.Clear();
            summaryErrors.Clear();
            showValidationErrors = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving paper: {Message}", ex.Message);
            summaryErrors.Add($"Error saving quiz: {ex.Message}");
        }
    }

    private void AddQuestion()
    {
        var question = new Question();
        questions.Add(question);
        questionAnswers[question] = new List<Answer>();
        AddAnswer(question);
    }

    private void RemoveQuestion(Question question)
    {
        questions.Remove(question);
        questionAnswers.Remove(question);
    }

    private void AddAnswer(Question question)
    {
        questionAnswers[question].Add(new Answer());
    }

    private void RemoveAnswer(Question question, Answer answer)
    {
        questionAnswers[question].Remove(answer);
    }
}