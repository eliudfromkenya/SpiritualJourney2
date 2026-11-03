using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the settings page, supporting theme palette and mode changes
/// and sign-out.
/// </summary>
public partial class SettingsPageModel : ViewModelBase
{
    private const string LoginRoute = "//login";

    private readonly IThemeService _themeService;
    private readonly IAuthService _authService;

    /// <summary>Gets or sets the selected color palette.</summary>
    [ObservableProperty]
    private ThemePalette _selectedPalette;

    /// <summary>Gets or sets the selected theme mode.</summary>
    [ObservableProperty]
    private ThemeMode _selectedMode;

    /// <summary>Gets or sets the currently authenticated user.</summary>
    [ObservableProperty]
    private AppUser? _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageModel"/> class.
    /// </summary>
    /// <param name="themeService">The theme service for applying palette and mode changes.</param>
    /// <param name="authService">The authentication service for the current user and sign-out.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public SettingsPageModel(IThemeService themeService, IAuthService authService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _themeService = themeService;
        _authService = authService;
    }

    /// <summary>Loads the current theme and user when the page appears.</summary>
    [RelayCommand]
    private void Appearing()
    {
        SelectedPalette = _themeService.ActivePalette;
        SelectedMode = _themeService.ActiveMode;
        CurrentUser = _authService.CurrentUser;
    }

    /// <summary>Applies the selected color palette to the application.</summary>
    [RelayCommand]
    private void PaletteChanged()
    {
        _themeService.ActivePalette = SelectedPalette;
        _themeService.ApplyTheme();
    }

    /// <summary>Applies the selected theme mode to the application.</summary>
    [RelayCommand]
    private void ModeChanged()
    {
        _themeService.ActiveMode = SelectedMode;
        _themeService.ApplyTheme();
    }

    /// <summary>Signs out the current user and navigates to the login page.</summary>
    [RelayCommand]
    private async Task SignOutAsync()
    {
        await RunBusyAsync(async () =>
        {
            await _authService.SignOutAsync().ConfigureAwait(false);
            await Shell.Current.GoToAsync(LoginRoute).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
