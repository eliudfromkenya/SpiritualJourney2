using Microsoft.Maui.Media;

namespace SpiritualJourney.Services;

/// <summary>
/// Provides text-to-speech playback using the platform speech synthesizer,
/// tracking whether audio is currently playing.
/// </summary>
public class TtsService : ITtsService
{
    private readonly SemaphoreSlim _playbackGate = new(1, 1);
    private bool _isPlaying;

    /// <inheritdoc/>
    public bool IsPlaying => _isPlaying;

    /// <inheritdoc/>
    public async Task SpeakAsync(string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        await _playbackGate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            _isPlaying = true;
            var options = new SpeakOptions();
            await TextToSpeech.Default.SpeakAsync(text, options, ct).ConfigureAwait(false);
        }
        finally
        {
            _isPlaying = false;
            _playbackGate.Release();
        }
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken ct = default)
    {
        await TextToSpeech.Default.CancelAsync(ct).ConfigureAwait(false);
        _isPlaying = false;
    }
}
