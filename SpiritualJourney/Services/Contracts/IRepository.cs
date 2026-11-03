using System.Linq.Expressions;
using SpiritualJourney.Core;

namespace SpiritualJourney.Services.Contracts;

/// <summary>
/// Generic repository contract for CRUD operations on entities inheriting from <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Gets all entities, optionally filtered.</summary>
    Task<List<T>> ListAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);

    /// <summary>Gets a single entity by its identifier.</summary>
    Task<T?> GetAsync(string id, CancellationToken ct = default);

    /// <summary>Saves (inserts or updates) an entity.</summary>
    Task<string> SaveAsync(T entity, CancellationToken ct = default);

    /// <summary>Deletes an entity.</summary>
    Task<int> DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>Checks whether any entity matches the predicate.</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
}
