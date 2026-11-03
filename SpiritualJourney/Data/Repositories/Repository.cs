using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SpiritualJourney.Data.Repositories;

/// <summary>
/// Generic EF Core repository providing asynchronous CRUD operations for entities
/// that inherit from <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">The entity type managed by the repository.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class.
    /// </summary>
    /// <param name="context">The application database context used for data access.</param>
    public Repository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
    {
        var query = _context.Set<T>().AsNoTracking();
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync(string id, CancellationToken ct = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<string> SaveAsync(T entity, CancellationToken ct = default)
    {
        var existing = await _context.Set<T>().FindAsync(new object[] { entity.Id }, ct).ConfigureAwait(false);
        if (existing is null)
        {
            _context.Set<T>().Add(entity);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Remove(entity);
        return await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _context.Set<T>().AnyAsync(predicate, ct).ConfigureAwait(false);
    }
}
