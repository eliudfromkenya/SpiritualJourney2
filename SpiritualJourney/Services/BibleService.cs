using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SpiritualJourney.Services;

/// <summary>
/// Provides bible-specific operations beyond generic CRUD: reference parsing,
/// full-text search, navigation, and persistence of notes, highlights, and memory verses.
/// </summary>
public partial class BibleService : IBibleService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BibleService"/> class.
    /// </summary>
    /// <param name="context">The application database context used for queries and persistence.</param>
    public BibleService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<BibleTranslation>> GetTranslationsAsync(CancellationToken ct = default)
    {
        return await _context.BibleTranslations
            .OrderBy(t => t.Name)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<BibleBook>> GetBooksAsync(string translationId, CancellationToken ct = default)
    {
        return await _context.BibleBooks
            .Where(b => b.TranslationId == translationId)
            .OrderBy(b => b.Order)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<BibleChapter>> GetChaptersAsync(string bookId, CancellationToken ct = default)
    {
        return await _context.BibleChapters
            .Where(c => c.BookId == bookId)
            .OrderBy(c => c.Number)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<BibleVerse>> GetVersesAsync(string chapterId, CancellationToken ct = default)
    {
        return await _context.BibleVerses
            .Include(v => v.Notes)
            .Include(v => v.Highlight)
            .Where(v => v.ChapterId == chapterId)
            .OrderBy(v => v.Number)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<BibleBook>> AutocompleteBooksAsync(string translationId, string partial, CancellationToken ct = default)
    {
        var lowerPartial = partial.ToLowerInvariant();
        return await _context.BibleBooks
            .Where(b => b.TranslationId == translationId)
            .Where(b => b.Name.ToLower().StartsWith(lowerPartial) || b.ShortName.ToLower().StartsWith(lowerPartial))
            .OrderBy(b => b.Order)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<(BibleChapter? Chapter, List<BibleVerse> Verses, string? Error)> ParseReferenceAsync(string reference, string translationId, CancellationToken ct = default)
    {
        var match = ReferenceRegex().Match(reference.Trim());
        if (!match.Success)
        {
            return (null, [], "Invalid reference format. Use 'Book Chapter:Verse' or 'Book Chapter:Verse-Verse'.");
        }
        if (!int.TryParse(match.Groups["chapter"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var chapterNumber) ||
            !int.TryParse(match.Groups["start"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var startVerse))
        {
            return (null, [], "Invalid chapter or verse number.");
        }
        var endVerse = match.Groups["end"].Success
            ? int.Parse(match.Groups["end"].Value, CultureInfo.InvariantCulture)
            : startVerse;

        var chapter = await FindChapterAsync(translationId, match.Groups["book"].Value, chapterNumber, ct).ConfigureAwait(false);
        if (chapter is null)
        {
            return (null, [], $"Could not find '{match.Groups["book"].Value} {chapterNumber}'.");
        }
        var verses = await LoadVersesForRangeAsync(chapter.Id, startVerse, endVerse, ct).ConfigureAwait(false);
        return (chapter, verses, null);
    }

    /// <inheritdoc/>
    public async Task<List<BibleSearchResult>> SearchAsync(string translationId, string query, CancellationToken ct = default)
    {
        var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            return [];
        }
        var verses = _context.BibleVerses
            .Where(v => v.Chapter!.Book!.TranslationId == translationId)
            .AsNoTracking();
        foreach (var word in words)
        {
            var pattern = $"%{word}%";
            verses = verses.Where(v => EF.Functions.Like(v.Text, pattern));
        }
        return await verses
            .Select(v => new BibleSearchResult
            {
                VerseId = v.Id,
                TranslationCode = v.Chapter!.Book!.Translation!.Code,
                BookName = v.Chapter!.Book!.Name,
                Chapter = v.Chapter!.Number,
                Verse = v.Number,
                Snippet = v.Text
            })
            .ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<int> SaveMemoryVersesAsync(IEnumerable<string> verseIds, string userId, CancellationToken ct = default)
    {
        var ids = verseIds.ToList();
        if (ids.Count == 0)
        {
            return 0;
        }
        var existing = await _context.MemoryVerses
            .Where(m => m.UserId == userId && ids.Contains(m.VerseId))
            .Select(m => m.VerseId)
            .ToListAsync(ct).ConfigureAwait(false);
        var toAdd = ids.Except(existing)
            .Select(verseId => new MemoryVerse { VerseId = verseId, UserId = userId })
            .ToList();
        _context.MemoryVerses.AddRange(toAdd);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return toAdd.Count;
    }

    /// <inheritdoc/>
    public async Task<int> SaveNotesAsync(IEnumerable<string> verseIds, string noteText, CancellationToken ct = default)
    {
        var ids = verseIds.ToList();
        if (ids.Count == 0)
        {
            return 0;
        }
        var notes = ids.Select(verseId => new VerseNote { VerseId = verseId, Text = noteText }).ToList();
        _context.VerseNotes.AddRange(notes);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return notes.Count;
    }

    /// <inheritdoc/>
    public async Task SaveHighlightAsync(string verseId, HighlightColor color, CancellationToken ct = default)
    {
        var existing = await _context.VerseHighlights
            .FirstOrDefaultAsync(h => h.VerseId == verseId, ct).ConfigureAwait(false);
        if (existing is null)
        {
            _context.VerseHighlights.Add(new VerseHighlight { VerseId = verseId, Color = color });
        }
        else
        {
            existing.Color = color;
        }
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task<BibleChapter?> FindChapterAsync(string translationId, string bookName, int chapterNumber, CancellationToken ct)
    {
        var lowerName = bookName.ToLowerInvariant();
        return await _context.BibleChapters
            .Include(c => c.Book)
            .Where(c => c.Book!.TranslationId == translationId)
            .Where(c => c.Book!.Name.ToLower() == lowerName || c.Book!.ShortName.ToLower() == lowerName)
            .Where(c => c.Number == chapterNumber)
            .FirstOrDefaultAsync(ct).ConfigureAwait(false);
    }

    private async Task<List<BibleVerse>> LoadVersesForRangeAsync(string chapterId, int startVerse, int endVerse, CancellationToken ct)
    {
        return await _context.BibleVerses
            .Where(v => v.ChapterId == chapterId && v.Number >= startVerse && v.Number <= endVerse)
            .OrderBy(v => v.Number)
            .AsNoTracking()
            .ToListAsync(ct).ConfigureAwait(false);
    }

    [GeneratedRegex(@"^(?<book>[A-Za-z]+)\s+(?<chapter>\d+):(?<start>\d+)(?:-(?<end>\d+))?$", RegexOptions.IgnoreCase)]
    private static partial Regex ReferenceRegex();
}
