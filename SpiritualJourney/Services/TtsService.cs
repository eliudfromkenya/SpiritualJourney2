using Microsoft.Maui.Media;

namespace SpiritualJourney.Services;

/// <summary>
/// Provides text-to-speech playback using the platform speech synthesizer,
/// tracking whether audio is currently playing.
/// </summary>
public class TtsService : ITtsService
{
    private readonly SemaphoreSlim _playbackGate = new(1, 1);
    private CancellationTokenSource? _cts;
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
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _isPlaying = true;
            await TextToSpeech.Default.SpeakAsync(text, null, _cts.Token).ConfigureAwait(false);
        }
        finally
        {
            _isPlaying = false;
            _cts?.Dispose();
            _cts = null;
            _playbackGate.Release();
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken ct = default)
    {
        _cts?.Cancel();
        _isPlaying = false;
        return Task.CompletedTask;
    }
}
