using Microsoft.EntityFrameworkCore;

namespace SpiritualJourney.Services;

/// <summary>
/// Provides spaced-repetition review operations for memory verses using
/// a simplified SM-2 algorithm to schedule the next review date.
/// </summary>
public class MemoryReviewService : IMemoryReviewService
{
    private const double MinimumEaseFactor = 1.3;
    private const double EasyEaseFactorDelta = 0.1;
    private const double HardEaseFactorDelta = 0.2;
    private const int FirstInterval = 1;
    private const int SecondInterval = 6;

    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryReviewService"/> class.
    /// </summary>
    /// <param name="context">The application database context used for memory verse data.</param>
    public MemoryReviewService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<MemoryVerse>> GetDueVersesAsync(string userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.MemoryVerses
            .Where(m => m.UserId == userId && m.NextReviewAtUtc <= now)
            .Include(m => m.Verse)
            .OrderBy(m => m.NextReviewAtUtc)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<MemoryVerse>> GetAllAsync(string userId, CancellationToken ct = default)
    {
        return await _context.MemoryVerses
            .Where(m => m.UserId == userId)
            .Include(m => m.Verse)
            .OrderBy(m => m.NextReviewAtUtc)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ReviewAsync(string memoryVerseId, ReviewDifficulty difficulty, CancellationToken ct = default)
    {
        var verse = await _context.MemoryVerses
            .FirstOrDefaultAsync(m => m.Id == memoryVerseId, ct).ConfigureAwait(false);
        if (verse is null)
        {
            return;
        }
        ApplySm2(verse, difficulty);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> GetStreakAsync(string userId, CancellationToken ct = default)
    {
        var nextDates = await _context.MemoryVerses
            .Where(m => m.UserId == userId && m.NextReviewAtUtc <= DateTime.UtcNow)
            .Select(m => m.NextReviewAtUtc)
            .ToListAsync(ct).ConfigureAwait(false);
        var reviewedDays = nextDates.Select(d => d.Date).ToHashSet();
        return CountConsecutiveDays(reviewedDays);
    }

    private static void ApplySm2(MemoryVerse verse, ReviewDifficulty difficulty)
    {
        verse.Repetitions++;
        if (difficulty == ReviewDifficulty.Easy)
        {
            verse.EaseFactor = Math.Max(MinimumEaseFactor, verse.EaseFactor + EasyEaseFactorDelta);
            verse.Interval = ComputeEasyInterval(verse.Interval, verse.EaseFactor);
        }
        else
        {
            verse.EaseFactor = Math.Max(MinimumEaseFactor, verse.EaseFactor - HardEaseFactorDelta);
            verse.Interval = FirstInterval;
        }
        verse.NextReviewAtUtc = DateTime.UtcNow.AddDays(verse.Interval);
        verse.UpdatedAtUtc = DateTime.UtcNow;
    }

    private static int ComputeEasyInterval(int currentInterval, double easeFactor)
    {
        if (currentInterval <= 0)
        {
            return FirstInterval;
        }
        if (currentInterval == FirstInterval)
        {
            return SecondInterval;
        }
        return (int)Math.Round(currentInterval * easeFactor);
    }

    private static int CountConsecutiveDays(HashSet<DateTime> reviewedDays)
    {
        var streak = 0;
        var day = DateTime.UtcNow.Date;
        while (reviewedDays.Contains(day))
        {
            streak++;
            day = day.AddDays(-1);
        }
        return streak;
    }
}
