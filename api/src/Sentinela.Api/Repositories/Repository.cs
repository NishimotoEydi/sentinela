using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Data;

namespace Sentinela.Api.Repositories;

/// <summary>Implementacao do <see cref="IRepository{T}"/> sobre o EF Core.</summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SentinelaDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(SentinelaDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking().ToListAsync(ct);

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await Set.FindAsync(new object?[] { id }, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        await Set.FindAsync(new object?[] { id }, ct) is not null;

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await Set.AddAsync(entity, ct);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);

    public IQueryable<T> Query() => Set.AsQueryable();
}
