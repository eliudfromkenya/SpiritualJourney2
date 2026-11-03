using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the hymns page, supporting CRUD operations, search,
/// and text-to-speech playback of hymn lyrics.
/// </summary>
public partial class HymnsPageModel : ViewModelBase
{
    private readonly IRepository<Hymn> _hymnRepository;
    private readonly ITtsService _ttsService;
    private List<Hymn> _allHymns = [];

    /// <summary>Gets or sets the hymns displayed in the list.</summary>
    [ObservableProperty]
    private List<Hymn> _hymns = [];

    /// <summary>Gets or sets the currently selected hymn.</summary>
    [ObservableProperty]
    private Hymn? _selectedHymn;

    /// <summary>Gets or sets the search query for filtering hymns.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>Gets or sets the title for a new hymn.</summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>Gets or sets the author for a new hymn.</summary>
    [ObservableProperty]
    private string _author = string.Empty;

    /// <summary>Gets or sets the comma-separated tags for a new hymn.</summary>
    [ObservableProperty]
    private string _tags = string.Empty;

    /// <summary>Gets or sets the lyric content for a new hymn.</summary>
    [ObservableProperty]
    private string _lyricContent = string.Empty;

    /// <summary>Gets or sets the language of the lyric for a new hymn.</summary>
    [ObservableProperty]
    private string _lyricLanguage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="HymnsPageModel"/> class.
    /// </summary>
    /// <param name="hymnRepository">The repository for hymn entities.</param>
    /// <param name="ttsService">The text-to-speech service for lyric playback.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public HymnsPageModel(IRepository<Hymn> hymnRepository, ITtsService ttsService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _hymnRepository = hymnRepository;
        _ttsService = ttsService;
    }

    /// <summary>Loads all hymns when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        await RunBusyAsync(LoadHymnsAsync).ConfigureAwait(false);
    }

    /// <summary>Filters the displayed hymns by the search query.</summary>
    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Hymns = _allHymns;
            return;
        }
        var query = SearchQuery.Trim();
        Hymns = _allHymns
            .Where(h => h.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                || h.Author.Contains(query, StringComparison.OrdinalIgnoreCase)
                || h.Tags.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>Saves a new hymn from the form fields and reloads the list.</summary>
    [RelayCommand]
    private async Task SaveHymnAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }
        var hymn = BuildHymn();
        await RunBusyAsync(async () =>
        {
            await _hymnRepository.SaveAsync(hymn).ConfigureAwait(false);
            await LoadHymnsAsync().ConfigureAwait(false);
            ResetForm();
        }).ConfigureAwait(false);
    }

    /// <summary>Deletes the selected hymn and reloads the list.</summary>
    [RelayCommand]
    private async Task DeleteHymnAsync()
    {
        if (SelectedHymn is null)
        {
            return;
        }
        var hymn = SelectedHymn;
        await RunBusyAsync(async () =>
        {
            await _hymnRepository.DeleteAsync(hymn).ConfigureAwait(false);
            await LoadHymnsAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Speaks the selected hymn's first lyric via text-to-speech.</summary>
    [RelayCommand]
    private async Task ListenAsync()
    {
        var text = SelectedHymn?.Lyrics.FirstOrDefault()?.Content;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        try
        {
            await _ttsService.SpeakAsync(text).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleError(ex);
        }
    }

    private async Task LoadHymnsAsync()
    {
        _allHymns = await _hymnRepository.ListAsync().ConfigureAwait(false);
        Hymns = _allHymns;
    }

    private Hymn BuildHymn()
    {
        var hymn = new Hymn
        {
            Title = Title,
            Author = Author,
            Tags = Tags
        };
        if (!string.IsNullOrWhiteSpace(LyricContent))
        {
            hymn.Lyrics.Add(new HymnLyric
            {
                Language = LyricLanguage,
                Content = LyricContent
            });
        }
        return hymn;
    }

    private void ResetForm()
    {
        Title = string.Empty;
        Author = string.Empty;
        Tags = string.Empty;
        LyricContent = string.Empty;
        LyricLanguage = string.Empty;
    }
}
