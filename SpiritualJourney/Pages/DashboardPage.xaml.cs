namespace SpiritualJourney.Pages;

/// <summary>
/// Dashboard page displaying aggregated KPIs, charts, and quick navigation links.
/// </summary>
public partial class DashboardPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardPage"/> class.
    /// </summary>
    /// <param name="model">The dashboard page model providing KPIs and navigation commands.</param>
    public DashboardPage(DashboardPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
