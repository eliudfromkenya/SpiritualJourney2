using CommunityToolkit.Mvvm.ComponentModel;

namespace SpiritualJourney.PageModels;

/// <summary>
/// Base class for all view models, providing common busy-state tracking
/// and centralized error handling for asynchronous operations.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    /// <summary>Gets or sets a value indicating whether the view model is performing a long-running operation.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets the error handler used to surface exceptions to the user.</summary>
    protected IErrorHandler ErrorHandler { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="errorHandler">The error handler for surfacing exceptions to the user.</param>
    protected ViewModelBase(IErrorHandler errorHandler)
    {
        ErrorHandler = errorHandler;
    }

    /// <summary>
    /// Executes the specified asynchronous action while toggling the busy state.
    /// Any exception is forwarded to <see cref="ErrorHandler"/>.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task RunBusyAsync(Func<Task> action)
    {
        IsBusy = true;
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
