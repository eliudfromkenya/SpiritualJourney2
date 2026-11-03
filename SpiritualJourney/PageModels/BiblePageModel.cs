using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the bible page, supporting translation/book/chapter navigation,
/// verse selection, search, reference parsing, notes, highlights, memory verses, and TTS.
/// </summary>
public partial class BiblePageModel : ViewModelBase
{
    private readonly IBibleService _bibleService;
    private readonly ITtsService _ttsService;
    private readonly IAuthService _authService;
    private List<BibleChapter> _chapterEntities = [];

    /// <summary>Gets or sets the available bible translations.</summary>
    [ObservableProperty]
    private List<BibleTranslation> _translations = [];

    /// <summary>Gets or sets the currently selected translation.</summary>
    [ObservableProperty]
    private BibleTranslation? _selectedTranslation;

    /// <summary>Gets or sets the books available for the selected translation.</summary>
    [ObservableProperty]
    private List<BibleBook> _books = [];

    /// <summary>Gets or sets the currently selected book.</summary>
    [ObservableProperty]
    private BibleBook? _selectedBook;

    /// <summary>Gets or sets the chapter numbers available for the selected book.</summary>
    [ObservableProperty]
    private List<int> _chapters = [];

    /// <summary>Gets or sets the currently selected chapter number.</summary>
    [ObservableProperty]
    private int _selectedChapter;

    /// <summary>Gets or sets the verses for the selected chapter.</summary>
    [ObservableProperty]
    private List<BibleVerse> _verses = [];

    /// <summary>Gets or sets the verses currently selected by the user.</summary>
    [ObservableProperty]
    private List<BibleVerse> _selectedVerses = [];

    /// <summary>Gets or sets the search query text.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>Gets or sets the full-text search results.</summary>
    [ObservableProperty]
    private List<BibleSearchResult> _searchResults = [];

    /// <summary>Gets or sets the bible reference input (e.g. "Romans 3:23-27").</summary>
    [ObservableProperty]
    private string _referenceInput = string.Empty;

    /// <summary>Gets or sets the error message to display for reference parsing failures.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiblePageModel"/> class.
    /// </summary>
    /// <param name="bibleService">The bible service for data operations.</param>
    /// <param name="ttsService">The text-to-speech service for verse playback.</param>
    /// <param name="authService">The authentication service for the current user.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public BiblePageModel(IBibleService bibleService, ITtsService ttsService, IAuthService authService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _bibleService = bibleService;
        _ttsService = ttsService;
        _authService = authService;
    }

    /// <summary>Loads translations and the initial book/chapter/verse hierarchy when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        await RunBusyAsync(async () =>
        {
            Translations = await _bibleService.GetTranslationsAsync().ConfigureAwait(false);
            SelectedTranslation = Translations.FirstOrDefault();
            if (SelectedTranslation is not null)
            {
                await LoadBooksAsync().ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    /// <summary>Loads books when the selected translation changes.</summary>
    [RelayCommand]
    private async Task TranslationChangedAsync()
    {
        if (SelectedTranslation is null)
        {
            return;
        }
        await RunBusyAsync(LoadBooksAsync).ConfigureAwait(false);
    }

    /// <summary>Loads chapters when the selected book changes.</summary>
    [RelayCommand]
    private async Task BookChangedAsync()
    {
        if (SelectedBook is null)
        {
            return;
        }
        await RunBusyAsync(LoadChaptersAsync).ConfigureAwait(false);
    }

    /// <summary>Loads verses when the selected chapter changes.</summary>
    [RelayCommand]
    private async Task ChapterChangedAsync()
    {
        if (SelectedChapter <= 0)
        {
            return;
        }
        await RunBusyAsync(LoadVersesAsync).ConfigureAwait(false);
    }

    /// <summary>Searches verses in the selected translation matching the query.</summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        if (SelectedTranslation is null || string.IsNullOrWhiteSpace(SearchQuery))
        {
            return;
        }
        await RunBusyAsync(async () =>
        {
            SearchResults = await _bibleService.SearchAsync(SelectedTranslation.Id, SearchQuery).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Parses the reference input and loads matching verses.</summary>
    [RelayCommand]
    private async Task ParseReferenceAsync()
    {
        if (SelectedTranslation is null || string.IsNullOrWhiteSpace(ReferenceInput))
        {
            return;
        }
        await RunBusyAsync(async () =>
        {
            var (chapter, verses, error) = await _bibleService
                .ParseReferenceAsync(ReferenceInput, SelectedTranslation.Id)
                .ConfigureAwait(false);
            if (error is not null)
            {
                ErrorMessage = error;
                return;
            }
            ErrorMessage = string.Empty;
            Verses = verses;
            SyncSelectedBook(chapter);
        }).ConfigureAwait(false);
    }

    /// <summary>Saves the selected verses as memory verses for the current user.</summary>
    [RelayCommand]
    private async Task SaveMemoryAsync()
    {
        var userId = _authService.CurrentUser?.Id;
        if (userId is null || SelectedVerses.Count == 0)
        {
            return;
        }
        var verseIds = SelectedVerses.Select(v => v.Id).ToList();
        await RunBusyAsync(async () =>
        {
            await _bibleService.SaveMemoryVersesAsync(verseIds, userId).ConfigureAwait(false);
            SelectedVerses = [];
        }).ConfigureAwait(false);
    }

    /// <summary>Saves a note for each selected verse.</summary>
    /// <param name="noteText">The note text to attach.</param>
    [RelayCommand]
    private async Task AddNoteAsync(string noteText)
    {
        if (SelectedVerses.Count == 0 || string.IsNullOrWhiteSpace(noteText))
        {
            return;
        }
        var verseIds = SelectedVerses.Select(v => v.Id).ToList();
        await RunBusyAsync(async () =>
        {
            await _bibleService.SaveNotesAsync(verseIds, noteText).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Applies a highlight of the specified color to each selected verse.</summary>
    /// <param name="color">The highlight color to apply.</param>
    [RelayCommand]
    private async Task SaveHighlightAsync(HighlightColor color)
    {
        if (SelectedVerses.Count == 0)
        {
            return;
        }
        var verseIds = SelectedVerses.Select(v => v.Id).ToList();
        await RunBusyAsync(async () =>
        {
            foreach (var verseId in verseIds)
            {
                await _bibleService.SaveHighlightAsync(verseId, color).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    /// <summary>Speaks the specified verse text via text-to-speech.</summary>
    /// <param name="verse">The verse to read aloud.</param>
    [RelayCommand]
    private async Task ListenVerseAsync(BibleVerse verse)
    {
        if (verse is null)
        {
            return;
        }
        try
        {
            await _ttsService.SpeakAsync(verse.Text).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleError(ex);
        }
    }

    private async Task LoadBooksAsync()
    {
        if (SelectedTranslation is null)
        {
            return;
        }
        Books = await _bibleService.GetBooksAsync(SelectedTranslation.Id).ConfigureAwait(false);
        SelectedBook = Books.FirstOrDefault();
        if (SelectedBook is not null)
        {
            await LoadChaptersAsync().ConfigureAwait(false);
        }
    }

    private async Task LoadChaptersAsync()
    {
        if (SelectedBook is null)
        {
            return;
        }
        _chapterEntities = await _bibleService.GetChaptersAsync(SelectedBook.Id).ConfigureAwait(false);
        Chapters = _chapterEntities.Select(c => c.Number).ToList();
        SelectedChapter = Chapters.FirstOrDefault();
        if (SelectedChapter > 0)
        {
            await LoadVersesAsync().ConfigureAwait(false);
        }
    }

    private async Task LoadVersesAsync()
    {
        var chapter = _chapterEntities.FirstOrDefault(c => c.Number == SelectedChapter);
        if (chapter is null)
        {
            return;
        }
        Verses = await _bibleService.GetVersesAsync(chapter.Id).ConfigureAwait(false);
    }

    private void SyncSelectedBook(BibleChapter? chapter)
    {
        if (chapter is null)
        {
            return;
        }
        SelectedBook = Books.FirstOrDefault(b => b.Id == chapter.BookId);
    }
}
