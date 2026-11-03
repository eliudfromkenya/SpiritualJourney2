using CommunityToolkit.Maui;

namespace SpiritualJourney.Services;

/// <summary>
/// Displays unhandled exceptions to the user as a modal alert, serialized
/// through a semaphore to avoid overlapping dialogs.
/// </summary>
public class ModalErrorHandler : IErrorHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <inheritdoc/>
    public void HandleError(Exception ex)
    {
        DisplayAlertAsync(ex).FireAndForgetSafeAsync();
    }

    private async Task DisplayAlertAsync(Exception ex)
    {
        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            if (Shell.Current is Shell shell)
            {
                await shell.DisplayAlertAsync("Error", ex.Message, "OK").ConfigureAwait(false);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
