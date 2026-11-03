using Microsoft.EntityFrameworkCore;

namespace SpiritualJourney.Services;

/// <summary>
/// Initializes the encrypted database by applying EF Core migrations and seeding
/// sample bible, hymn, prayer, and task data on first run.
/// </summary>
public class DatabaseInitializer : IDatabaseInitializer
{
    private static readonly string[] GenesisChapter1Verses =
    {
        "In the beginning God created the heaven and the earth.",
        "And the earth was without form, and void; and darkness was upon the face of the deep.",
        "And the Spirit of God moved upon the face of the waters."
    };

    private static readonly string[] JohnChapter1Verses =
    {
        "In the beginning was the Word, and the Word was with God, and the Word was God."
    };

    private static readonly string[] JohnChapter3Verses =
    {
        "For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life."
    };

    private static readonly string[] RomansChapter3Verses =
    {
        "For all have sinned, and come short of the glory of God;",
        "Being justified freely by his grace through the redemption that is in Christ Jesus:",
        "Whom God hath set forth to be a propitiation through faith in his blood, to declare his righteousness for the remission of sins that are past, through the forbearance of God;",
        "To declare, I say, at this time his righteousness: that he might be just, and the justifier of him which believeth in Jesus.",
        "Where is boasting then? It is excluded. By what law? of works? Nay: but by the law of faith."
    };

    private readonly AppDbContext _context;
    private bool _isSeeded;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseInitializer"/> class.
    /// </summary>
    /// <param name="context">The application database context to initialize and seed.</param>
    public DatabaseInitializer(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public bool IsSeeded => _isSeeded;

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _context.Database.MigrateAsync(ct).ConfigureAwait(false);
        _isSeeded = await _context.BibleTranslations.AnyAsync(ct).ConfigureAwait(false);
        if (_isSeeded)
        {
            return;
        }
        await SeedAsync(ct).ConfigureAwait(false);
        _isSeeded = true;
    }

    private async Task SeedAsync(CancellationToken ct)
    {
        SeedBible();
        SeedHymn();
        SeedPrayer();
        SeedTask();
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private void SeedBible()
    {
        var kjv = new BibleTranslation { Name = "King James Version", Code = "KJV", Language = "English" };
        _context.BibleTranslations.Add(kjv);

        var genesis = CreateBook(kjv.Id, "Genesis", "Gen", 1, "Old");
        var john = CreateBook(kjv.Id, "John", "John", 4, "New");
        var romans = CreateBook(kjv.Id, "Romans", "Rom", 6, "New");
        _context.BibleBooks.AddRange(genesis, john, romans);

        AddChapterWithVerses(genesis.Id, 1, GenesisChapter1Verses);
        AddChapterWithVerses(john.Id, 1, JohnChapter1Verses);
        AddChapterWithVerses(john.Id, 3, JohnChapter3Verses);
        AddChapterWithVerses(romans.Id, 3, RomansChapter3Verses);
    }

    private void SeedHymn()
    {
        var hymn = new Hymn { Title = "Amazing Grace", Author = "John Newton", Tags = "grace,salvation,hymn" };
        _context.Hymns.Add(hymn);
        _context.HymnLyrics.Add(new HymnLyric
        {
            HymnId = hymn.Id,
            Language = "English",
            Content = "Amazing grace! how sweet the sound, that saved a wretch like me!"
        });
    }

    private void SeedPrayer()
    {
        _context.Prayers.Add(new Prayer
        {
            Title = "The Lord's Prayer",
            Content = "Our Father which art in heaven, Hallowed be thy name.",
            Category = "Devotional",
            Tags = "lord,prayer,model"
        });
    }

    private void SeedTask()
    {
        _context.Tasks.Add(new SpiritualTask
        {
            Title = "Read one chapter daily",
            Description = "Commit to reading at least one bible chapter each day.",
            Status = TaskStatus.Backlog,
            Priority = TaskPriority.Medium,
            Category = "Bible Reading"
        });
    }

    private void AddChapterWithVerses(string bookId, int chapterNumber, string[] verseTexts)
    {
        var chapter = new BibleChapter { BookId = bookId, Number = chapterNumber };
        _context.BibleChapters.Add(chapter);
        for (var i = 0; i < verseTexts.Length; i++)
        {
            _context.BibleVerses.Add(new BibleVerse
            {
                ChapterId = chapter.Id,
                Number = i + 1,
                Text = verseTexts[i]
            });
        }
    }

    private static BibleBook CreateBook(string translationId, string name, string shortName, int order, string testament)
        => new()
        {
            TranslationId = translationId,
            Name = name,
            ShortName = shortName,
            Order = order,
            Testament = testament
        };
}
