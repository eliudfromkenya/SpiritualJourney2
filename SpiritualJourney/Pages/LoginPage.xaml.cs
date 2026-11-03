namespace SpiritualJourney.Pages;

/// <summary>
/// Login page providing email, Google, and Facebook sign-in entry points.
/// </summary>
public partial class LoginPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    /// <param name="model">The login page model providing sign-in commands.</param>
    public LoginPage(LoginPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
