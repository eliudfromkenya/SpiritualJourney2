using Microsoft.EntityFrameworkCore;
using SpiritualJourney.Models;

namespace SpiritualJourney.Data;

/// <summary>
/// The EF Core DbContext for the Spiritual Journey app, backed by an encrypted SQLite (SQLCipher) database.
/// All entity identifiers and foreign keys are strings.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Bible translations table.</summary>
    public DbSet<BibleTranslation> BibleTranslations => Set<BibleTranslation>();

    /// <summary>Bible books table.</summary>
    public DbSet<BibleBook> BibleBooks => Set<BibleBook>();

    /// <summary>Bible chapters table.</summary>
    public DbSet<BibleChapter> BibleChapters => Set<BibleChapter>();

    /// <summary>Bible verses table.</summary>
    public DbSet<BibleVerse> BibleVerses => Set<BibleVerse>();

    /// <summary>Verse notes table.</summary>
    public DbSet<VerseNote> VerseNotes => Set<VerseNote>();

    /// <summary>Verse highlights table.</summary>
    public DbSet<VerseHighlight> VerseHighlights => Set<VerseHighlight>();

    /// <summary>Memory verses table.</summary>
    public DbSet<MemoryVerse> MemoryVerses => Set<MemoryVerse>();

    /// <summary>Hymns table.</summary>
    public DbSet<Hymn> Hymns => Set<Hymn>();

    /// <summary>Hymn lyrics table.</summary>
    public DbSet<HymnLyric> HymnLyrics => Set<HymnLyric>();

    /// <summary>Prayers table.</summary>
    public DbSet<Prayer> Prayers => Set<Prayer>();

    /// <summary>Spiritual tasks table.</summary>
    public DbSet<SpiritualTask> Tasks => Set<SpiritualTask>();

    /// <summary>Sermons table.</summary>
    public DbSet<Sermon> Sermons => Set<Sermon>();

    /// <summary>Sermon translations table.</summary>
    public DbSet<SermonTranslation> SermonTranslations => Set<SermonTranslation>();

    /// <summary>Venues table.</summary>
    public DbSet<Venue> Venues => Set<Venue>();

    /// <summary>Sermon-venue links table.</summary>
    public DbSet<SermonVenue> SermonVenues => Set<SermonVenue>();

    /// <summary>Users table.</summary>
    public DbSet<AppUser> Users => Set<AppUser>();

    /// <summary>Auth sessions table.</summary>
    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    /// <summary>
    /// Configures the entity model, relationships, and string-key conventions.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBibleEntities(modelBuilder);
        ConfigureHymnEntities(modelBuilder);
        ConfigurePrayerEntities(modelBuilder);
        ConfigureTaskEntities(modelBuilder);
        ConfigureSermonEntities(modelBuilder);
        ConfigureUserEntities(modelBuilder);
    }

    private static void ConfigureBibleEntities(ModelBuilder modelBuilder)
    {
        var translation = modelBuilder.Entity<BibleTranslation>();
        translation.HasKey(e => e.Id);
        translation.Property(e => e.Name).IsRequired();
        translation.Property(e => e.Code).IsRequired();
        translation.HasMany(e => e.Books).WithOne(b => b.Translation!)
            .HasForeignKey(b => b.TranslationId).OnDelete(DeleteBehavior.Cascade);

        var book = modelBuilder.Entity<BibleBook>();
        book.HasKey(e => e.Id);
        book.Property(e => e.Name).IsRequired();
        book.HasMany(e => e.Chapters).WithOne(c => c.Book!)
            .HasForeignKey(c => c.BookId).OnDelete(DeleteBehavior.Cascade);

        var chapter = modelBuilder.Entity<BibleChapter>();
        chapter.HasKey(e => e.Id);
        chapter.HasMany(e => e.Verses).WithOne(v => v.Chapter!)
            .HasForeignKey(v => v.ChapterId).OnDelete(DeleteBehavior.Cascade);

        var verse = modelBuilder.Entity<BibleVerse>();
        verse.HasKey(e => e.Id);
        verse.Property(e => e.Text).IsRequired();
        verse.HasMany(e => e.Notes).WithOne(n => n.Verse!)
            .HasForeignKey(n => n.VerseId).OnDelete(DeleteBehavior.Cascade);
        verse.HasOne(e => e.Highlight).WithOne(h => h.Verse!)
            .HasForeignKey<VerseHighlight>(h => h.VerseId).OnDelete(DeleteBehavior.Cascade);

        var note = modelBuilder.Entity<VerseNote>();
        note.HasKey(e => e.Id);
        note.Property(e => e.Text).IsRequired();

        var highlight = modelBuilder.Entity<VerseHighlight>();
        highlight.HasKey(e => e.Id);

        var memory = modelBuilder.Entity<MemoryVerse>();
        memory.HasKey(e => e.Id);
        memory.HasOne(e => e.Verse).WithMany().HasForeignKey(e => e.VerseId).OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureHymnEntities(ModelBuilder modelBuilder)
    {
        var hymn = modelBuilder.Entity<Hymn>();
        hymn.HasKey(e => e.Id);
        hymn.Property(e => e.Title).IsRequired();
        hymn.HasMany(e => e.Lyrics).WithOne(l => l.Hymn!)
            .HasForeignKey(l => l.HymnId).OnDelete(DeleteBehavior.Cascade);

        var lyric = modelBuilder.Entity<HymnLyric>();
        lyric.HasKey(e => e.Id);
        lyric.Property(e => e.Content).IsRequired();
    }

    private static void ConfigurePrayerEntities(ModelBuilder modelBuilder)
    {
        var prayer = modelBuilder.Entity<Prayer>();
        prayer.HasKey(e => e.Id);
        prayer.Property(e => e.Title).IsRequired();
        prayer.Property(e => e.Content).IsRequired();
    }

    private static void ConfigureTaskEntities(ModelBuilder modelBuilder)
    {
        var task = modelBuilder.Entity<SpiritualTask>();
        task.HasKey(e => e.Id);
        task.Property(e => e.Title).IsRequired();
    }

    private static void ConfigureSermonEntities(ModelBuilder modelBuilder)
    {
        var sermon = modelBuilder.Entity<Sermon>();
        sermon.HasKey(e => e.Id);
        sermon.Property(e => e.Title).IsRequired();
        sermon.HasMany(e => e.Translations).WithOne(t => t.Sermon!)
            .HasForeignKey(t => t.SermonId).OnDelete(DeleteBehavior.Cascade);
        sermon.HasMany(e => e.Venues).WithOne(sv => sv.Sermon!)
            .HasForeignKey(sv => sv.SermonId).OnDelete(DeleteBehavior.Cascade);

        var translation = modelBuilder.Entity<SermonTranslation>();
        translation.HasKey(e => e.Id);
        translation.Property(e => e.Title).IsRequired();
        translation.Property(e => e.Content).IsRequired();

        var venue = modelBuilder.Entity<Venue>();
        venue.HasKey(e => e.Id);
        venue.Property(e => e.Name).IsRequired();

        var sermonVenue = modelBuilder.Entity<SermonVenue>();
        sermonVenue.HasKey(e => e.Id);
        sermonVenue.HasOne(e => e.Venue).WithMany().HasForeignKey(e => e.VenueId).OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureUserEntities(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<AppUser>();
        user.HasKey(e => e.Id);
        user.Property(e => e.Email).IsRequired();

        var session = modelBuilder.Entity<AuthSession>();
        session.HasKey(e => e.Id);
        session.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
