namespace SpiritualJourney.Pages;

/// <summary>
/// Prayers page supporting CRUD operations, search, and TTS playback of prayer content.
/// </summary>
public partial class PrayersPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrayersPage"/> class.
    /// </summary>
    /// <param name="model">The prayers page model providing CRUD and search commands.</param>
    public PrayersPage(PrayersPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
