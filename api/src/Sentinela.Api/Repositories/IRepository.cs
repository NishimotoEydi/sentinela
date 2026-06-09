namespace Sentinela.Api.Repositories;

/// <summary>
/// Repositorio generico (camada de acesso a dados). Abstrai o EF Core dos services,
/// permitindo trocar a persistencia sem tocar na regra de negocio.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>Consulta componivel (filtros, Include, ordenacao) para os services.</summary>
    IQueryable<T> Query();
}
