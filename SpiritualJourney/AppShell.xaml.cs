using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace SpiritualJourney;

/// <summary>
/// The application shell providing flyout navigation to all modules.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppShell"/> class.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();
        Routing.SetRoute(this, "//login");
    }

    /// <summary>
    /// Displays a snackbar with the specified message.
    /// </summary>
    public static async Task DisplaySnackbarAsync(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FF3300"),
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = new CornerRadius(0),
            Font = Font.SystemFontOfSize(18),
            ActionButtonFont = Font.SystemFontOfSize(14)
        };

        var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
        await snackbar.Show(cancellationTokenSource.Token);
    }

    /// <summary>
    /// Displays a toast with the specified message (not supported on Windows).
    /// </summary>
    public static async Task DisplayToastAsync(string message)
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var toast = Toast.Make(message, textSize: 18);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await toast.Show(cts.Token);
    }
}
