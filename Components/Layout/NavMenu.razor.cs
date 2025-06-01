namespace test.Components.Layout;
using Microsoft.AspNetCore.Components.Authorization;

public partial class NavMenu
{
    private string navButtonClass = "text-slate-100 text-xl hover:bg-slate-100 hover:text-slate-950 p-3 rounded transition-all duration-500 flex items-center space-x-3";
    private string currentTime = "";
    private string currentUsername = "";
    private Timer? timer;

    protected override async Task OnInitializedAsync()
    {
        UpdateTime();
        timer = new Timer(UpdateTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        await UpdateUsername();

        // Subscribe to authentication state changes
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        await UpdateUsername();
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateUsername()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            currentUsername = user.Identity.Name ?? "";
        }
        else
        {
            currentUsername = "";
        }
    }

    private void UpdateTime(object? state = null)
    {
        var istTime = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        currentTime = istTime.ToString("HH:mm");
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        timer?.Dispose();
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

}