using Microsoft.EntityFrameworkCore;

namespace SpiritualJourney.Services;

/// <summary>
/// Aggregates KPI and chart data for the dashboard from all modules in a single call.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardService"/> class.
    /// </summary>
    /// <param name="context">The application database context used for aggregation queries.</param>
    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<DashboardData> LoadAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var data = new DashboardData
        {
            TranslationCount = await _context.BibleTranslations.CountAsync(ct).ConfigureAwait(false),
            VerseCount = await _context.BibleVerses.CountAsync(ct).ConfigureAwait(false),
            MemoryDueCount = await _context.MemoryVerses.CountAsync(m => m.NextReviewAtUtc <= now, ct).ConfigureAwait(false),
            MemoryTotalCount = await _context.MemoryVerses.CountAsync(ct).ConfigureAwait(false),
            OpenTaskCount = await _context.Tasks.CountAsync(t => t.Status != TaskStatus.Done, ct).ConfigureAwait(false),
            CompletedTaskCount = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.Done, ct).ConfigureAwait(false),
            SermonCount = await _context.Sermons.CountAsync(ct).ConfigureAwait(false),
            HymnCount = await _context.Hymns.CountAsync(ct).ConfigureAwait(false),
            PrayerCount = await _context.Prayers.CountAsync(ct).ConfigureAwait(false)
        };
        data.MemoryStreak = await ComputeStreakAsync(ct).ConfigureAwait(false);
        data.TaskStatusDistribution = await GetTaskStatusDistributionAsync(ct).ConfigureAwait(false);
        data.SermonDeliveryTrend = await GetSermonDeliveryTrendAsync(ct).ConfigureAwait(false);
        return data;
    }

    private async Task<int> ComputeStreakAsync(CancellationToken ct)
    {
        var nextDates = await _context.MemoryVerses
            .Where(m => m.NextReviewAtUtc <= DateTime.UtcNow)
            .Select(m => m.NextReviewAtUtc)
            .ToListAsync(ct).ConfigureAwait(false);
        var reviewedDays = nextDates.Select(d => d.Date).ToHashSet();
        return CountConsecutiveDays(reviewedDays);
    }

    private async Task<List<DashboardChartItem>> GetTaskStatusDistributionAsync(CancellationToken ct)
    {
        var statuses = await _context.Tasks.Select(t => t.Status).ToListAsync(ct).ConfigureAwait(false);
        return statuses
            .GroupBy(s => s)
            .OrderBy(g => g.Key)
            .Select(g => new DashboardChartItem(g.Key.ToString(), g.Count()))
            .ToList();
    }

    private async Task<List<DashboardChartItem>> GetSermonDeliveryTrendAsync(CancellationToken ct)
    {
        var dates = await _context.Sermons
            .Where(s => s.DeliveryDate.HasValue)
            .Select(s => s.DeliveryDate!.Value)
            .ToListAsync(ct).ConfigureAwait(false);
        return dates
            .GroupBy(d => d.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(g => new DashboardChartItem(g.Key, g.Count()))
            .ToList();
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
