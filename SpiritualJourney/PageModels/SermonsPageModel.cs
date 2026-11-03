using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the sermons page, supporting CRUD operations, venue selection,
/// and search for sermon entries.
/// </summary>
public partial class SermonsPageModel : ViewModelBase
{
    private readonly IRepository<Sermon> _sermonRepository;
    private readonly IRepository<Venue> _venueRepository;
    private List<Sermon> _allSermons = [];

    /// <summary>Gets or sets the sermons displayed in the list.</summary>
    [ObservableProperty]
    private List<Sermon> _sermons = [];

    /// <summary>Gets or sets the currently selected sermon.</summary>
    [ObservableProperty]
    private Sermon? _selectedSermon;

    /// <summary>Gets or sets the available venues.</summary>
    [ObservableProperty]
    private List<Venue> _venues = [];

    /// <summary>Gets or sets the title for a new sermon.</summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>Gets or sets the content for a new sermon.</summary>
    [ObservableProperty]
    private string _content = string.Empty;

    /// <summary>Gets or sets the comma-separated tags for a new sermon.</summary>
    [ObservableProperty]
    private string _tags = string.Empty;

    /// <summary>Gets or sets the delivery date for a new sermon.</summary>
    [ObservableProperty]
    private DateTime? _deliveryDate;

    /// <summary>Gets or sets the venue selected for a new sermon.</summary>
    [ObservableProperty]
    private Venue? _selectedVenue;

    /// <summary>Gets or sets the search query for filtering sermons.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SermonsPageModel"/> class.
    /// </summary>
    /// <param name="sermonRepository">The repository for sermon entities.</param>
    /// <param name="venueRepository">The repository for venue entities.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public SermonsPageModel(IRepository<Sermon> sermonRepository, IRepository<Venue> venueRepository, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _sermonRepository = sermonRepository;
        _venueRepository = venueRepository;
    }

    /// <summary>Loads all sermons and venues when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        await RunBusyAsync(LoadSermonsAsync).ConfigureAwait(false);
    }

    /// <summary>Saves a new sermon from the form fields and reloads the list.</summary>
    [RelayCommand]
    private async Task SaveSermonAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }
        var sermon = BuildSermon();
        await RunBusyAsync(async () =>
        {
            await _sermonRepository.SaveAsync(sermon).ConfigureAwait(false);
            await LoadSermonsAsync().ConfigureAwait(false);
            ResetForm();
        }).ConfigureAwait(false);
    }

    /// <summary>Deletes the selected sermon and reloads the list.</summary>
    [RelayCommand]
    private async Task DeleteSermonAsync()
    {
        if (SelectedSermon is null)
        {
            return;
        }
        var sermon = SelectedSermon;
        await RunBusyAsync(async () =>
        {
            await _sermonRepository.DeleteAsync(sermon).ConfigureAwait(false);
            await LoadSermonsAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>Filters the displayed sermons by the search query.</summary>
    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Sermons = _allSermons;
            return;
        }
        var query = SearchQuery.Trim();
        Sermons = _allSermons
            .Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                || s.Content.Contains(query, StringComparison.OrdinalIgnoreCase)
                || s.Tags.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private async Task LoadSermonsAsync()
    {
        _allSermons = await _sermonRepository.ListAsync().ConfigureAwait(false);
        Sermons = _allSermons;
        Venues = await _venueRepository.ListAsync().ConfigureAwait(false);
    }

    private Sermon BuildSermon()
    {
        var sermon = new Sermon
        {
            Title = Title,
            Content = Content,
            Tags = Tags,
            DeliveryDate = DeliveryDate
        };
        if (SelectedVenue is not null)
        {
            sermon.Venues.Add(new SermonVenue { VenueId = SelectedVenue.Id });
        }
        return sermon;
    }

    private void ResetForm()
    {
        Title = string.Empty;
        Content = string.Empty;
        Tags = string.Empty;
        DeliveryDate = null;
        SelectedVenue = null;
    }
}
