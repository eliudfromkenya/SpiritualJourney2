namespace SpiritualJourney.Pages;

/// <summary>
/// Hymns page supporting CRUD operations, search, and TTS playback of hymn lyrics.
/// </summary>
public partial class HymnsPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HymnsPage"/> class.
    /// </summary>
    /// <param name="model">The hymns page model providing CRUD and search commands.</param>
    public HymnsPage(HymnsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
