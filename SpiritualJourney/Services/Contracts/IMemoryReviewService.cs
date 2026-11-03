using SpiritualJourney.Core.Enums;
using SpiritualJourney.Models;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Provides spaced-repetition review operations for memory verses.
/// </summary>
public interface IMemoryReviewService
{
    /// <summary>Lists memory verses that are due for review.</summary>
    Task<List<MemoryVerse>> GetDueVersesAsync(string userId, CancellationToken ct = default);

    /// <summary>Lists all memory verses for a user.</summary>
    Task<List<MemoryVerse>> GetAllAsync(string userId, CancellationToken ct = default);

    /// <summary>Records a review and updates ease factor, interval, and next review date.</summary>
    Task ReviewAsync(string memoryVerseId, ReviewDifficulty difficulty, CancellationToken ct = default);

    /// <summary>Computes the current review streak in days.</summary>
    Task<int> GetStreakAsync(string userId, CancellationToken ct = default);
}
