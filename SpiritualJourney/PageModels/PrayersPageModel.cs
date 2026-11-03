using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the prayers page, supporting CRUD operations, search,
/// and text-to-speech playback of prayer content.
/// </summary>
public partial class PrayersPageModel : ViewModelBase
{
    private readonly IRepository<Prayer> _prayerRepository;
    private readonly ITtsService _ttsService;
    private List<Prayer> _allPrayers = [];

    /// <summary>Gets or sets the prayers displayed in the list.</summary>
    [ObservableProperty]
    private List<Prayer> _prayers = [];

    /// <summary>Gets or sets the currently selected prayer.</summary>
    [ObservableProperty]
    private Prayer? _selectedPrayer;

    /// <summary>Gets or sets the search query for filtering prayers.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>Gets or sets the title for a new prayer.</summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>Gets or sets the content for a new prayer.</summary>
    [ObservableProperty]
    private string _content = string.Empty;

    /// <summary>Gets or sets the category for a new prayer.</summary>
    [ObservableProperty]
    private string _category = string.Empty;

    /// <summary>Gets or sets the comma-separated tags for a new prayer.</summary>
    [ObservableProperty]
    private string _tags = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrayersPageModel"/> class.
    /// </summary>
    /// <param name="prayerRepository">The repository for prayer entities.</param>
    /// <param name="ttsService">The text-to-speech service for prayer playback.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public PrayersPageModel(IRepository<Prayer> prayerRepository, ITtsService ttsService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _prayerRepository = prayerRepository;
        _ttsService = ttsService;
    }

    /// <summary>Loads all prayers when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        await RunBusyAsync(LoadPrayersAsync).ConfigureAwait(false);
    }

    /// <summary>Filters the displayed prayers by the search query.</summary>
    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Prayers = _allPrayers;
            return;
        }
        var query = SearchQuery.Trim();
        Prayers = _allPrayers
            .Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Content.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Tags.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>Saves a new prayer from the form fields and reloads the list.</summary>
    [RelayCommand]
    private async Task SavePrayerAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }
        var prayer = BuildPrayer();
        await RunBusyAsync(async () =>
        {
            await _prayerRepository.SaveAsync(prayer).ConfigureAwait(false);
            await LoadPrayersAsync().ConfigureAwait(false);
            ResetForm();
        }).ConfigureAwait(false);
    }

    /// <summary>Deletes the selected prayer and reloads the list.</summary>
    [RelayCommand]
    private async Task DeletePrayerAsync()
    {
        if (SelectedPrayer is null)
        {
            return;
        }
        var prayer = SelectedPrayer;
        await RunBusyAsync(async () =>
        {
            await _prayerRepository.DeleteAsync(prayer).ConfigureAwait(false);
            await LoadPrayersAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Speaks the selected prayer's content via text-to-speech.</summary>
    [RelayCommand]
    private async Task ListenAsync()
    {
        var text = SelectedPrayer?.Content;
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

    private async Task LoadPrayersAsync()
    {
        _allPrayers = await _prayerRepository.ListAsync().ConfigureAwait(false);
        Prayers = _allPrayers;
    }

    private Prayer BuildPrayer()
    {
        return new Prayer
        {
            Title = Title,
            Content = Content,
            Category = Category,
            Tags = Tags
        };
    }

    private void ResetForm()
    {
        Title = string.Empty;
        Content = string.Empty;
        Category = string.Empty;
        Tags = string.Empty;
    }
}
