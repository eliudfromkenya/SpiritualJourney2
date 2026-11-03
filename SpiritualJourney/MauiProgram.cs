using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpiritualJourney.Data;
using SpiritualJourney.Data.Repositories;
using SpiritualJourney.Models;
using SpiritualJourney.Services;
using SpiritualJourney.Services.Contracts;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SpiritualJourney;

/// <summary>
/// Configures the MAUI application, services, and dependency injection container.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the <see cref="MauiApp"/>.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if WINDOWS
                Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
                {
                    handler.PlatformView.SingleSelectionFollowsFocus = false;
                });
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        // Database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(Constants.ConnectionString));

        // Repositories (generic repository for each entity)
        RegisterRepository<BibleTranslation>(builder.Services);
        RegisterRepository<BibleBook>(builder.Services);
        RegisterRepository<BibleChapter>(builder.Services);
        RegisterRepository<BibleVerse>(builder.Services);
        RegisterRepository<VerseNote>(builder.Services);
        RegisterRepository<VerseHighlight>(builder.Services);
        RegisterRepository<MemoryVerse>(builder.Services);
        RegisterRepository<Hymn>(builder.Services);
        RegisterRepository<HymnLyric>(builder.Services);
        RegisterRepository<Prayer>(builder.Services);
        RegisterRepository<SpiritualTask>(builder.Services);
        RegisterRepository<Sermon>(builder.Services);
        RegisterRepository<SermonTranslation>(builder.Services);
        RegisterRepository<Venue>(builder.Services);
        RegisterRepository<SermonVenue>(builder.Services);
        RegisterRepository<AppUser>(builder.Services);
        RegisterRepository<AuthSession>(builder.Services);

        // Services
        builder.Services.AddSingleton<ITtsService, TtsService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IBibleService, BibleService>();
        builder.Services.AddSingleton<IMemoryReviewService, MemoryReviewService>();
        builder.Services.AddSingleton<IDashboardService, DashboardService>();
        builder.Services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        builder.Services.AddSingleton<IErrorHandler, ModalErrorHandler>();
        builder.Services.AddSingleton<ModalErrorHandler>();

        // Page models (singleton for shell pages, transient for detail pages)
        builder.Services.AddSingleton<LoginPageModel>();
        builder.Services.AddSingleton<DashboardPageModel>();
        builder.Services.AddSingleton<BiblePageModel>();
        builder.Services.AddSingleton<MemoryReviewPageModel>();
        builder.Services.AddSingleton<HymnsPageModel>();
        builder.Services.AddSingleton<TasksPageModel>();
        builder.Services.AddSingleton<SermonsPageModel>();
        builder.Services.AddSingleton<PrayersPageModel>();
        builder.Services.AddSingleton<SettingsPageModel>();

        // Pages (transient - resolved via Shell routing)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<BiblePage>();
        builder.Services.AddTransient<MemoryReviewPage>();
        builder.Services.AddTransient<HymnsPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<SermonsPage>();
        builder.Services.AddTransient<PrayersPage>();
        builder.Services.AddTransient<SettingsPage>();

        var app = builder.Build();
        App.Services = app.Services;
        return app;
    }

    /// <summary>
    /// Registers a generic repository for the specified entity type.
    /// </summary>
    private static void RegisterRepository<T>(IServiceCollection services) where T : BaseEntity
    {
        services.AddSingleton<IRepository<T>, Repository<T>>();
    }
}
