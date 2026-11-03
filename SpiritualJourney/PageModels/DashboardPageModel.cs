using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpiritualJourney.PageModels;

/// <summary>
/// View model for the dashboard page, displaying aggregated KPIs and chart data
/// alongside quick navigation to the main modules.
/// </summary>
public partial class DashboardPageModel : ViewModelBase
{
    private const string BibleRoute = "//bible";
    private const string TasksRoute = "//tasks";
    private const string MemoryRoute = "//memory";
    private const string TodayDateFormat = "D";

    private readonly IDashboardService _dashboardService;
    private readonly IAuthService _authService;

    /// <summary>Gets or sets the aggregated dashboard data.</summary>
    [ObservableProperty]
    private DashboardData? _dashboardData;

    /// <summary>Gets or sets a value indicating whether the list is being refreshed via pull-to-refresh.</summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>Gets or sets the formatted today date string.</summary>
    [ObservableProperty]
    private string _today = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardPageModel"/> class.
    /// </summary>
    /// <param name="dashboardService">The dashboard service for loading aggregated data.</param>
    /// <param name="authService">The authentication service for checking session state.</param>
    /// <param name="errorHandler">The error handler for surfacing exceptions.</param>
    public DashboardPageModel(IDashboardService dashboardService, IAuthService authService, IErrorHandler errorHandler)
        : base(errorHandler)
    {
        _dashboardService = dashboardService;
        _authService = authService;
    }

    /// <summary>Loads dashboard data when the page appears.</summary>
    [RelayCommand]
    private async Task AppearingAsync()
    {
        Today = DateTime.Today.ToString(TodayDateFormat);
        if (!_authService.IsAuthenticated)
        {
            return;
        }
        await LoadDashboardAsync().ConfigureAwait(false);
    }

    /// <summary>Refreshes the dashboard data via pull-to-refresh.</summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            await LoadDashboardAsync().ConfigureAwait(false);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>Navigates to the bible page.</summary>
    [RelayCommand]
    private async Task NavigateToBibleAsync()
    {
        await Shell.Current.GoToAsync(BibleRoute).ConfigureAwait(false);
    }

    /// <summary>Navigates to the tasks page.</summary>
    [RelayCommand]
    private async Task NavigateToTasksAsync()
    {
        await Shell.Current.GoToAsync(TasksRoute).ConfigureAwait(false);
    }

    /// <summary>Navigates to the memory review page.</summary>
    [RelayCommand]
    private async Task NavigateToMemoryAsync()
    {
        await Shell.Current.GoToAsync(MemoryRoute).ConfigureAwait(false);
    }

    private async Task LoadDashboardAsync()
    {
        await RunBusyAsync(async () =>
        {
            DashboardData = await _dashboardService.LoadAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
