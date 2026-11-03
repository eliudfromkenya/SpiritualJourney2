using Microsoft.Extensions.DependencyInjection;

namespace SpiritualJourney;

/// <summary>
/// The application entry point. Initializes the database, theme, and session on startup.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets or sets the service provider used for startup initialization.
    /// Set by <see cref="MauiProgram.CreateMauiApp"/> after the DI container is built.
    /// </summary>
    internal static IServiceProvider? Services { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Creates the main window and runs startup initialization (database, theme, session).
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
        _ = InitializeStartupAsync();
        return window;
    }

    /// <summary>
    /// Runs startup initialization: applies migrations, seeds data, loads theme, and restores session.
    /// </summary>
    private static async Task InitializeStartupAsync()
    {
        if (Services is null)
        {
            return;
        }

        try
        {
            var dbInitializer = Services.GetRequiredService<IDatabaseInitializer>();
            await dbInitializer.InitializeAsync().ConfigureAwait(false);

            var themeService = Services.GetRequiredService<IThemeService>();
            themeService.LoadAndApply();

            var authService = Services.GetRequiredService<IAuthService>();
            bool restored = await authService.RestoreSessionAsync().ConfigureAwait(false);

            if (restored)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync("//dashboard"));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Startup initialization failed: {ex}");
        }
    }
}
