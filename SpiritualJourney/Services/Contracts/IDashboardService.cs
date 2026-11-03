using SpiritualJourney.Models;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Aggregates KPI and chart data for the dashboard from all modules.
/// </summary>
public interface IDashboardService
{
    /// <summary>Loads all dashboard insights in a single call.</summary>
    Task<DashboardData> LoadAsync(CancellationToken ct = default);
}
