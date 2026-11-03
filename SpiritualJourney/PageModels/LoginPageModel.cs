using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the login page, supporting email, Google, and Facebook sign-in.
/// </summary>
public partial class LoginPageModel : ViewModelBase
{
    private const string DashboardRoute = "//dashboard";
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IAuthService _authService;

    /// <summary>Gets or sets the email address entered by the user.</summary>
    [ObservableProperty]
    private string _email = string.Empty;

    /// <summary>Gets or sets the password entered by the user.</summary>
    [ObservableProperty]
    private string _password = string.Empty;

    /// <summary>Gets or sets the error message to display when sign-in fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPageModel"/> class.
    /// </summary>
    /// <param name="authService">The authentication service for sign-in operations.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public LoginPageModel(IAuthService authService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _authService = authService;
    }

    /// <summary>Signs in with email and password, navigating to the dashboard on success.</summary>
    [RelayCommand]
    private async Task SignInAsync()
    {
        ErrorMessage = string.Empty;
        await RunBusyAsync(async () =>
        {
            var success = await _authService.SignInWithEmailAsync(Email, Password).ConfigureAwait(false);
            if (success)
            {
                await Shell.Current.GoToAsync(DashboardRoute).ConfigureAwait(false);
            }
            else
            {
                ErrorMessage = InvalidCredentialsMessage;
            }
        }).ConfigureAwait(false);
    }

    /// <summary>Starts the Google sign-in flow, navigating to the dashboard on success.</summary>
    [RelayCommand]
    private async Task SignInWithGoogleAsync()
    {
        ErrorMessage = string.Empty;
        await RunBusyAsync(async () =>
        {
            var success = await _authService.SignInWithGoogleAsync().ConfigureAwait(false);
            if (success)
            {
                await Shell.Current.GoToAsync(DashboardRoute).ConfigureAwait(false);
            }
            else
            {
                ErrorMessage = InvalidCredentialsMessage;
            }
        }).ConfigureAwait(false);
    }

    /// <summary>Starts the Facebook sign-in flow, navigating to the dashboard on success.</summary>
    [RelayCommand]
    private async Task SignInWithFacebookAsync()
    {
        ErrorMessage = string.Empty;
        await RunBusyAsync(async () =>
        {
            var success = await _authService.SignInWithFacebookAsync().ConfigureAwait(false);
            if (success)
            {
                await Shell.Current.GoToAsync(DashboardRoute).ConfigureAwait(false);
            }
            else
            {
                ErrorMessage = InvalidCredentialsMessage;
            }
        }).ConfigureAwait(false);
    }
}
