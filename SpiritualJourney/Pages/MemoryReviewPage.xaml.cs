namespace SpiritualJourney.Pages;

/// <summary>
/// Memory review page supporting spaced-repetition review of due memory verses.
/// </summary>
public partial class MemoryReviewPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryReviewPage"/> class.
    /// </summary>
    /// <param name="model">The memory review page model providing review commands.</param>
    public MemoryReviewPage(MemoryReviewPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
