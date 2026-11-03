namespace SpiritualJourney.Pages;

/// <summary>
/// Sermons page supporting CRUD operations, venue selection, and search for sermon entries.
/// </summary>
public partial class SermonsPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SermonsPage"/> class.
    /// </summary>
    /// <param name="model">The sermons page model providing CRUD and search commands.</param>
    public SermonsPage(SermonsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
