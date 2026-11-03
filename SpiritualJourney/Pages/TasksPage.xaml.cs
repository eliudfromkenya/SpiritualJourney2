namespace SpiritualJourney.Pages;

/// <summary>
/// Tasks page supporting CRUD operations, filtering, and completion toggling for spiritual tasks.
/// </summary>
public partial class TasksPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TasksPage"/> class.
    /// </summary>
    /// <param name="model">The tasks page model providing CRUD and filter commands.</param>
    public TasksPage(TasksPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    /// <summary>Sets the filter to All and invokes the filter changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFilterAllClicked(object? sender, EventArgs e)
    {
        if (BindingContext is TasksPageModel model)
        {
            model.ActiveFilter = TaskFilter.All;
            ExecuteFilterChanged(model);
        }
    }

    /// <summary>Sets the filter to Today and invokes the filter changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFilterTodayClicked(object? sender, EventArgs e)
    {
        if (BindingContext is TasksPageModel model)
        {
            model.ActiveFilter = TaskFilter.Today;
            ExecuteFilterChanged(model);
        }
    }

    /// <summary>Sets the filter to Week and invokes the filter changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFilterWeekClicked(object? sender, EventArgs e)
    {
        if (BindingContext is TasksPageModel model)
        {
            model.ActiveFilter = TaskFilter.Week;
            ExecuteFilterChanged(model);
        }
    }

    /// <summary>Sets the filter to Overdue and invokes the filter changed command.</summary>
    /// <param name="sender">The source button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFilterOverdueClicked(object? sender, EventArgs e)
    {
        if (BindingContext is TasksPageModel model)
        {
            model.ActiveFilter = TaskFilter.Overdue;
            ExecuteFilterChanged(model);
        }
    }

    private static void ExecuteFilterChanged(TasksPageModel model)
    {
        if (model.FilterChangedCommand.CanExecute(null))
        {
            model.FilterChangedCommand.Execute(null);
        }
    }
}
