namespace SpiritualJourney.Data;

/// <summary>
/// Provides database-related constants such as the database filename and encryption key.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The encrypted SQLite database filename.
    /// </summary>
    public const string DatabaseFilename = "SpiritualJourney.db3";

    /// <summary>
    /// The SQLCipher encryption key used to protect the local database.
    /// In production this should be stored in secure storage; this default
    /// is used for initial development and seeding.
    /// </summary>
    public const string DatabaseEncryptionKey = "SJ-SQLCipher-2026-SecureKey-9f3a7c1e";

    /// <summary>
    /// Gets the full database path inside the app's data directory.
    /// </summary>
    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    /// <summary>
    /// Gets the EF Core connection string with SQLCipher encryption enabled.
    /// </summary>
    public static string ConnectionString =>
        $"Data Source={DatabasePath};Password={DatabaseEncryptionKey};";
}
