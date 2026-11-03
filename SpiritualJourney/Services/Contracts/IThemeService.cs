using SpiritualJourney.Core.Enums;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Manages the active color palette and light/dark mode, persisting user preferences.
/// </summary>
public interface IThemeService
{
    /// <summary>Gets or sets the active color palette.</summary>
    ThemePalette ActivePalette { get; set; }

    /// <summary>Gets or sets the active theme mode (light or dark).</summary>
    ThemeMode ActiveMode { get; set; }

    /// <summary>Applies the current palette and mode to the application resources.</summary>
    void ApplyTheme();

    /// <summary>Loads saved theme preferences and applies them.</summary>
    void LoadAndApply();
}
