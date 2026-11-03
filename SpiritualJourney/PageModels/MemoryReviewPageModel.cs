using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the memory review page, supporting spaced-repetition review
/// of due memory verses and text-to-speech playback.
/// </summary>
public partial class MemoryReviewPageModel : ViewModelBase
{
    private readonly IMemoryReviewService _reviewService;
    private readonly ITtsService _ttsService;
    private readonly IAuthService _authService;

    /// <summary>Gets or sets the memory verses due for review.</summary>
    [ObservableProperty]
    private List<MemoryVerse> _dueVerses = [];

    /// <summary>Gets or sets all memory verses for the current user.</summary>
    [ObservableProperty]
    private List<MemoryVerse> _allVerses = [];

    /// <summary>Gets or sets the memory verse currently being reviewed.</summary>
    [ObservableProperty]
    private MemoryVerse? _currentVerse;

    /// <summary>Gets or sets the current review streak in days.</summary>
    [ObservableProperty]
    private int _streak;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryReviewPageModel"/> class.
    /// </summary>
    /// <param name="reviewService">The memory review service for spaced-repetition operations.</param>
    /// <param name="ttsService">The text-to-speech service for verse playback.</param>
    /// <param name="authService">The authentication service for the current user.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public MemoryReviewPageModel(IMemoryReviewService reviewService, ITtsService ttsService, IAuthService authService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _reviewService = reviewService;
        _ttsService = ttsService;
        _authService = authService;
    }

    /// <summary>Loads due verses, all verses, and the streak when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        var userId = _authService.CurrentUser?.Id;
        if (userId is null)
        {
            return;
        }
        await RunBusyAsync(async () =>
        {
            DueVerses = await _reviewService.GetDueVersesAsync(userId).ConfigureAwait(false);
            AllVerses = await _reviewService.GetAllAsync(userId).ConfigureAwait(false);
            Streak = await _reviewService.GetStreakAsync(userId).ConfigureAwait(false);
            CurrentVerse = DueVerses.FirstOrDefault();
        }).ConfigureAwait(false);
    }

    /// <summary>Records an "easy" review for the current verse and advances to the next.</summary>
    [RelayCommand]
    private async Task ReviewEasyAsync()
    {
        await ReviewCurrentAsync(ReviewDifficulty.Easy).ConfigureAwait(false);
    }

    /// <summary>Records a "hard" review for the current verse and advances to the next.</summary>
    [RelayCommand]
    private async Task ReviewHardAsync()
    {
        await ReviewCurrentAsync(ReviewDifficulty.Hard).ConfigureAwait(false);
    }

    /// <summary>Speaks the current verse text via text-to-speech.</summary>
    [RelayCommand]
    private async Task ListenAsync()
    {
        var text = CurrentVerse?.Verse?.Text;
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

    private async Task ReviewCurrentAsync(ReviewDifficulty difficulty)
    {
        if (CurrentVerse is null)
        {
            return;
        }
        var verseId = CurrentVerse.Id;
        await RunBusyAsync(async () =>
        {
            await _reviewService.ReviewAsync(verseId, difficulty).ConfigureAwait(false);
            DueVerses = DueVerses.Where(v => v.Id != verseId).ToList();
            CurrentVerse = DueVerses.FirstOrDefault();
        }).ConfigureAwait(false);
    }
}
