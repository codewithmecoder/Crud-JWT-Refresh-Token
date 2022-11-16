using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.RepositoryInterfaces;

namespace Ticket.Tracking.System.Repositories;

public class BaseEfRepository<T> : IAsyncRepository<T> where T : BaseModel
{
    protected readonly AppDbContext _dbContext;
    public BaseEfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken: ct);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Set<T>().ToListAsync(ct);
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
    }
}
