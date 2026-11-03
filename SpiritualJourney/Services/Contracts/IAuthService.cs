using SpiritualJourney.Core.Enums;
using SpiritualJourney.Models;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Provides authentication operations for email, Google, and Facebook sign-in.
/// </summary>
public interface IAuthService
{
    /// <summary>Gets the currently authenticated user, if any.</summary>
    AppUser? CurrentUser { get; }

    /// <summary>Gets a value indicating whether a valid session exists.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Signs in with email and password.</summary>
    Task<bool> SignInWithEmailAsync(string email, string password, CancellationToken ct = default);

    /// <summary>Starts the Google sign-in flow.</summary>
    Task<bool> SignInWithGoogleAsync(CancellationToken ct = default);

    /// <summary>Starts the Facebook sign-in flow.</summary>
    Task<bool> SignInWithFacebookAsync(CancellationToken ct = default);

    /// <summary>Restores a previously persisted session if the token is still valid.</summary>
    Task<bool> RestoreSessionAsync(CancellationToken ct = default);

    /// <summary>Signs out and clears the persisted session.</summary>
    Task SignOutAsync(CancellationToken ct = default);

    /// <summary>Creates a new user account with email and password, then signs them in.</summary>
    /// <returns><c>true</c> if registration succeeded; <c>false</c> if the email is already registered.</returns>
    Task<bool> RegisterAsync(string displayName, string email, string password, CancellationToken ct = default);

    /// <summary>Resets the password for the account associated with the given email.</summary>
    /// <returns><c>true</c> if the password was reset; <c>false</c> if no account exists for the email.</returns>
    Task<bool> ResetPasswordAsync(string email, string newPassword, CancellationToken ct = default);
}
