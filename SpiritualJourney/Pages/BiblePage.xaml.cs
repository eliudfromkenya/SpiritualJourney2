namespace SpiritualJourney.Pages;

/// <summary>
/// Bible page supporting translation/book/chapter navigation, verse selection,
/// search, reference parsing, notes, highlights, memory verses, and TTS.
/// </summary>
public partial class BiblePage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiblePage"/> class.
    /// </summary>
    /// <param name="model">The bible page model providing navigation and verse commands.</param>
    public BiblePage(BiblePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    /// <summary>Invokes the translation changed command when the translation picker selection changes.</summary>
    /// <param name="sender">The source picker.</param>
    /// <param name="e">The event arguments.</param>
    private void OnTranslationChanged(object? sender, EventArgs e)
    {
        if (BindingContext is BiblePageModel model && model.TranslationChangedCommand.CanExecute(null))
        {
            model.TranslationChangedCommand.Execute(null);
        }
    }

    /// <summary>Invokes the book changed command when the book picker selection changes.</summary>
    /// <param name="sender">The source picker.</param>
    /// <param name="e">The event arguments.</param>
    private void OnBookChanged(object? sender, EventArgs e)
    {
        if (BindingContext is BiblePageModel model && model.BookChangedCommand.CanExecute(null))
        {
            model.BookChangedCommand.Execute(null);
        }
    }

    /// <summary>Invokes the chapter changed command when the chapter picker selection changes.</summary>
    /// <param name="sender">The source picker.</param>
    /// <param name="e">The event arguments.</param>
    private void OnChapterChanged(object? sender, EventArgs e)
    {
        if (BindingContext is BiblePageModel model && model.ChapterChangedCommand.CanExecute(null))
        {
            model.ChapterChangedCommand.Execute(null);
        }
    }

    /// <summary>Syncs the selected verses from the CollectionView to the page model.</summary>
    /// <param name="sender">The source CollectionView.</param>
    /// <param name="e">The selection changed event arguments.</param>
    private void OnVersesSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is BiblePageModel model)
        {
            model.SelectedVerses = e.CurrentSelection.OfType<BibleVerse>().ToList();
        }
    }
}
