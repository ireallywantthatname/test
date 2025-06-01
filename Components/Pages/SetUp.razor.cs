using Test.Types;
namespace test.Components.Pages;
using Microsoft.AspNetCore.WebUtilities;

public partial class SetUp
{
    private bool IsStarted = false;
    private readonly List<Choice> Choices = new List<Choice>();

    protected override void OnInitialized()
    {
        Choices.Add(new Choice
        {
            Title = "For Student",
            Details = "Register here to establish your student account. This will enable you to access the online examination platform, participate in assessments, track your progress, and utilize educational resources.",
            EventHandler = () => Navigation.NavigateTo("/register?role=student")
        });

        Choices.Add(new Choice
        {
            Title = "For Educators",
            Details = "Register here to create your educator account. This will allow you to design and administer courses, configure assessments, and manage student interactions.",
            EventHandler = () => Navigation.NavigateTo("/register?role=educator")
        });

        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("started", out var startedParam) &&
            startedParam == "true")
        {
            IsStarted = true;
        }
    }
}