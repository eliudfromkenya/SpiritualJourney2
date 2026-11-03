namespace SpiritualJourney.Pages;

/// <summary>
/// Settings page supporting theme palette and mode changes and sign-out.
/// </summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    /// <param name="model">The settings page model providing theme and sign-out commands.</param>
    public SettingsPage(SettingsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    /// <summary>Sets the palette to Blue and invokes the palette changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPaletteBlueClicked(object? sender, EventArgs e)
    {
        ApplyPalette(ThemePalette.Blue);
    }

    /// <summary>Sets the palette to Green and invokes the palette changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPaletteGreenClicked(object? sender, EventArgs e)
    {
        ApplyPalette(ThemePalette.Green);
    }

    /// <summary>Sets the palette to Pink and invokes the palette changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPalettePinkClicked(object? sender, EventArgs e)
    {
        ApplyPalette(ThemePalette.Pink);
    }

    /// <summary>Sets the palette to White and invokes the palette changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPaletteWhiteClicked(object? sender, EventArgs e)
    {
        ApplyPalette(ThemePalette.White);
    }

    /// <summary>Sets the mode to Light and invokes the mode changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnModeLightClicked(object? sender, EventArgs e)
    {
        ApplyMode(ThemeMode.Light);
    }

    /// <summary>Sets the mode to Dark and invokes the mode changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnModeDarkClicked(object? sender, EventArgs e)
    {
        ApplyMode(ThemeMode.Dark);
    }

    private void ApplyPalette(ThemePalette palette)
    {
        if (BindingContext is SettingsPageModel model)
        {
            model.SelectedPalette = palette;
            if (model.PaletteChangedCommand.CanExecute(null))
            {
                model.PaletteChangedCommand.Execute(null);
            }
        }
    }

    private void ApplyMode(ThemeMode mode)
    {
        if (BindingContext is SettingsPageModel model)
        {
            model.SelectedMode = mode;
            if (model.ModeChangedCommand.CanExecute(null))
            {
                model.ModeChangedCommand.Execute(null);
            }
        }
    }
}
