namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Provides text-to-speech playback for verses, prayers, and memory verses.
/// </summary>
public interface ITtsService
{
    /// <summary>Gets a value indicating whether playback is currently active.</summary>
    bool IsPlaying { get; }

    /// <summary>Starts speaking the specified text.</summary>
    Task SpeakAsync(string text, CancellationToken ct = default);

    /// <summary>Stops any ongoing playback.</summary>
    Task StopAsync(CancellationToken ct = default);
}
