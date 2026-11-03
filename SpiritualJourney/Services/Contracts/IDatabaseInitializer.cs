namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Initializes the encrypted database: applies migrations and seeds bible data on first run.
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>Gets a value indicating whether the database has been seeded.</summary>
    bool IsSeeded { get; }

    /// <summary>Applies migrations and seeds initial data if needed.</summary>
    Task InitializeAsync(CancellationToken ct = default);
}
