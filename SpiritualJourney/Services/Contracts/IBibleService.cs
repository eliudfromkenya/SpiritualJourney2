using SpiritualJourney.Core.Enums;
using SpiritualJourney.Models;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Provides bible-specific operations beyond generic CRUD: reference parsing, search, and navigation.
/// </summary>
public interface IBibleService
{
    /// <summary>Lists all available translations.</summary>
    Task<List<BibleTranslation>> GetTranslationsAsync(CancellationToken ct = default);

    /// <summary>Lists books for a translation.</summary>
    Task<List<BibleBook>> GetBooksAsync(string translationId, CancellationToken ct = default);

    /// <summary>Lists chapters for a book.</summary>
    Task<List<BibleChapter>> GetChaptersAsync(string bookId, CancellationToken ct = default);

    /// <summary>Lists verses for a chapter, including notes and highlights.</summary>
    Task<List<BibleVerse>> GetVersesAsync(string chapterId, CancellationToken ct = default);

    /// <summary>Auto-completes book names from a partial input (e.g. "Ge" -> Genesis).</summary>
    Task<List<BibleBook>> AutocompleteBooksAsync(string translationId, string partial, CancellationToken ct = default);

    /// <summary>Parses a reference like "Romans 3:23-27" and loads the matching verses.</summary>
    Task<(BibleChapter? Chapter, List<BibleVerse> Verses, string? Error)> ParseReferenceAsync(string reference, string translationId, CancellationToken ct = default);

    /// <summary>Full-text searches across all verses in a translation.</summary>
    Task<List<BibleSearchResult>> SearchAsync(string translationId, string query, CancellationToken ct = default);

    /// <summary>Saves selected verses as memory verses, preventing duplicates.</summary>
    Task<int> SaveMemoryVersesAsync(IEnumerable<string> verseIds, string userId, CancellationToken ct = default);

    /// <summary>Saves a note for each selected verse.</summary>
    Task<int> SaveNotesAsync(IEnumerable<string> verseIds, string noteText, CancellationToken ct = default);

    /// <summary>Saves or updates a highlight for a verse.</summary>
    Task SaveHighlightAsync(string verseId, HighlightColor color, CancellationToken ct = default);
}
