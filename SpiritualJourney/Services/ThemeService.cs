using Microsoft.Maui.Graphics;

namespace SpiritualJourney.Services;

/// <summary>
/// Manages the active color palette and light/dark theme mode, persisting
/// user preferences and applying the selected theme to application resources.
/// </summary>
public class ThemeService : IThemeService
{
    private const string ActivePaletteKey = "theme_active_palette";
    private const string ActiveModeKey = "theme_active_mode";

    private const string DarkBackgroundHex = "#17171A";
    private const string LightBackgroundHex = "#FFFFFF";
    private const string LightSecondaryBackgroundHex = "#F5F5F5";
    private const string DarkSecondaryBackgroundHex = "#1E1E1E";

    private ThemePalette _activePalette = ThemePalette.Blue;
    private ThemeMode _activeMode = ThemeMode.Light;

    /// <inheritdoc/>
    public ThemePalette ActivePalette
    {
        get => _activePalette;
        set
        {
            _activePalette = value;
            Preferences.Set(ActivePaletteKey, (int)value);
        }
    }

    /// <inheritdoc/>
    public ThemeMode ActiveMode
    {
        get => _activeMode;
        set
        {
            _activeMode = value;
            Preferences.Set(ActiveModeKey, (int)value);
        }
    }

    /// <inheritdoc/>
    public void ApplyTheme()
    {
        if (Application.Current is null)
        {
            return;
        }
        var resources = Application.Current.Resources;
        var (primary, primaryDark, secondary, tertiary) = GetPaletteColors();
        Application.Current.UserAppTheme = ActiveMode == ThemeMode.Dark ? AppTheme.Dark : AppTheme.Light;
        resources["Primary"] = primary;
        resources["PrimaryDark"] = primaryDark;
        resources["Secondary"] = secondary;
        resources["Tertiary"] = tertiary;
        resources["DarkBackground"] = Color.FromArgb(DarkBackgroundHex);
        resources["LightBackground"] = Color.FromArgb(LightBackgroundHex);
        resources["LightSecondaryBackground"] = Color.FromArgb(LightSecondaryBackgroundHex);
        resources["DarkSecondaryBackground"] = Color.FromArgb(DarkSecondaryBackgroundHex);
    }

    /// <inheritdoc/>
    public void LoadAndApply()
    {
        _activePalette = (ThemePalette)Preferences.Get(ActivePaletteKey, (int)ThemePalette.Blue);
        _activeMode = (ThemeMode)Preferences.Get(ActiveModeKey, (int)ThemeMode.Light);
        ApplyTheme();
    }

    private (Color Primary, Color PrimaryDark, Color Secondary, Color Tertiary) GetPaletteColors()
    {
        var (lightPrimary, darkPrimary, secondary, tertiary) = ActivePalette switch
        {
            ThemePalette.Blue => (Color.FromArgb("#2196F3"), Color.FromArgb("#0D47A1"), Color.FromArgb("#BBDEFB"), Color.FromArgb("#1565C0")),
            ThemePalette.Green => (Color.FromArgb("#4CAF50"), Color.FromArgb("#1B5E20"), Color.FromArgb("#C8E6C9"), Color.FromArgb("#2E7D32")),
            ThemePalette.Pink => (Color.FromArgb("#E91E63"), Color.FromArgb("#880E4F"), Color.FromArgb("#F8BBD0"), Color.FromArgb("#C2185B")),
            ThemePalette.White => (Color.FromArgb("#607D8B"), Color.FromArgb("#000000"), Color.FromArgb("#ECEFF1"), Color.FromArgb("#455A64")),
            _ => (Color.FromArgb("#2196F3"), Color.FromArgb("#0D47A1"), Color.FromArgb("#BBDEFB"), Color.FromArgb("#1565C0"))
        };
        var primary = ActiveMode == ThemeMode.Dark ? darkPrimary : lightPrimary;
        return (primary, darkPrimary, secondary, tertiary);
    }
}
