using System.Security.Cryptography;
using System.Text;

namespace SpiritualJourney.Services;

/// <summary>
/// Provides authentication operations for email, Google, and Facebook sign-in,
/// persisting sessions for restoration across app launches.
/// </summary>
public class AuthService : IAuthService
{
    private const string SessionTokenKey = "auth_session_token";
    private const string SessionUserIdKey = "auth_session_user_id";
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(30);

    private const string GoogleProviderUserId = "google-simulated-user";
    private const string GoogleDisplayName = "Google User";
    private const string GoogleEmail = "google.user@spiritualjourney.app";

    private const string FacebookProviderUserId = "facebook-simulated-user";
    private const string FacebookDisplayName = "Facebook User";
    private const string FacebookEmail = "facebook.user@spiritualjourney.app";

    private readonly IRepository<AppUser> _users;
    private readonly IRepository<AuthSession> _sessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="users">The repository for <see cref="AppUser"/> entities.</param>
    /// <param name="sessions">The repository for <see cref="AuthSession"/> entities.</param>
    public AuthService(IRepository<AppUser> users, IRepository<AuthSession> sessions)
    {
        _users = users;
        _sessions = sessions;
    }

    /// <inheritdoc/>
    public AppUser? CurrentUser { get; private set; }

    /// <inheritdoc/>
    public bool IsAuthenticated => CurrentUser is not null;

    /// <inheritdoc/>
    public async Task<bool> SignInWithEmailAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await FindUserByEmailAsync(email, ct).ConfigureAwait(false);
        if (user is null || !VerifyPassword(password, user.PasswordHash))
        {
            return false;
        }
        await CreateSessionAsync(user, ct).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc/>
    public Task<bool> SignInWithGoogleAsync(CancellationToken ct = default)
        => SignInWithProviderAsync(AuthProvider.Google, GoogleProviderUserId, GoogleDisplayName, GoogleEmail, ct);

    /// <inheritdoc/>
    public Task<bool> SignInWithFacebookAsync(CancellationToken ct = default)
        => SignInWithProviderAsync(AuthProvider.Facebook, FacebookProviderUserId, FacebookDisplayName, FacebookEmail, ct);

    /// <inheritdoc/>
    public async Task<bool> RestoreSessionAsync(CancellationToken ct = default)
    {
        var token = Preferences.Get(SessionTokenKey, string.Empty);
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        var session = await FindSessionByTokenAsync(token, ct).ConfigureAwait(false);
        if (session is null || session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            ClearPersistedSession();
            return false;
        }
        var user = await _users.GetAsync(session.UserId, ct).ConfigureAwait(false);
        if (user is null)
        {
            return false;
        }
        CurrentUser = user;
        return true;
    }

    /// <inheritdoc/>
    public async Task SignOutAsync(CancellationToken ct = default)
    {
        var token = Preferences.Get(SessionTokenKey, string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            await DeleteSessionAsync(token, ct).ConfigureAwait(false);
        }
        ClearPersistedSession();
        CurrentUser = null;
    }

    private async Task<AppUser?> FindUserByEmailAsync(string email, CancellationToken ct)
    {
        var users = await _users.ListAsync(u => u.Email == email, ct).ConfigureAwait(false);
        return users.FirstOrDefault();
    }

    private async Task CreateSessionAsync(AppUser user, CancellationToken ct)
    {
        var session = new AuthSession
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.Add(SessionLifetime)
        };
        await _sessions.SaveAsync(session, ct).ConfigureAwait(false);
        Preferences.Set(SessionTokenKey, session.Token);
        Preferences.Set(SessionUserIdKey, user.Id);
        CurrentUser = user;
    }

    private async Task<bool> SignInWithProviderAsync(AuthProvider provider, string providerUserId, string displayName, string email, CancellationToken ct)
    {
        var existing = await _users.ListAsync(u => u.Provider == provider && u.ProviderUserId == providerUserId, ct).ConfigureAwait(false);
        var user = existing.FirstOrDefault();
        if (user is null)
        {
            user = new AppUser
            {
                Provider = provider,
                ProviderUserId = providerUserId,
                DisplayName = displayName,
                Email = email
            };
            await _users.SaveAsync(user, ct).ConfigureAwait(false);
        }
        await CreateSessionAsync(user, ct).ConfigureAwait(false);
        return true;
    }

    private async Task<AuthSession?> FindSessionByTokenAsync(string token, CancellationToken ct)
    {
        var sessions = await _sessions.ListAsync(s => s.Token == token, ct).ConfigureAwait(false);
        return sessions.FirstOrDefault();
    }

    private async Task DeleteSessionAsync(string token, CancellationToken ct)
    {
        var sessions = await _sessions.ListAsync(s => s.Token == token, ct).ConfigureAwait(false);
        foreach (var session in sessions)
        {
            await _sessions.DeleteAsync(session, ct).ConfigureAwait(false);
        }
    }

    private static void ClearPersistedSession()
    {
        Preferences.Remove(SessionTokenKey);
        Preferences.Remove(SessionUserIdKey);
    }

    private static bool VerifyPassword(string password, string storedHash)
        => !string.IsNullOrEmpty(storedHash) && HashPassword(password) == storedHash;

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
