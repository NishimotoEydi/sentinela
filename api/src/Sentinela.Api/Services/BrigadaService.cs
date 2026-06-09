using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IBrigadaService
{
    Task<IReadOnlyList<BrigadaResponse>> GetAllAsync(StatusBrigada? status, CancellationToken ct = default);
    Task<BrigadaResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BrigadaResponse> CreateAsync(BrigadaCreateRequest request, CancellationToken ct = default);
    Task<BrigadaResponse> UpdateStatusAsync(int id, StatusBrigada status, CancellationToken ct = default);
}

public class BrigadaService : IBrigadaService
{
    private readonly IRepository<Brigada> _brigadas;

    public BrigadaService(IRepository<Brigada> brigadas)
    {
        _brigadas = brigadas;
    }

    public async Task<IReadOnlyList<BrigadaResponse>> GetAllAsync(StatusBrigada? status, CancellationToken ct = default)
    {
        var query = _brigadas.Query().AsNoTracking();
        if (status is not null)
        {
            query = query.Where(b => b.Status == status);
        }

        var lista = await query.OrderBy(b => b.Nome).ToListAsync(ct);
        return lista.Select(b => b.ToResponse()).ToList();
    }

    public async Task<BrigadaResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var brigada = await _brigadas.GetByIdAsync(id, ct)
                      ?? throw NotFoundException.For("Brigada", id);
        return brigada.ToResponse();
    }

    public async Task<BrigadaResponse> CreateAsync(BrigadaCreateRequest request, CancellationToken ct = default)
    {
        var brigada = request.ToEntity();
        await _brigadas.AddAsync(brigada, ct);
        await _brigadas.SaveChangesAsync(ct);
        return brigada.ToResponse();
    }

    public async Task<BrigadaResponse> UpdateStatusAsync(int id, StatusBrigada status, CancellationToken ct = default)
    {
        var brigada = await _brigadas.GetByIdAsync(id, ct)
                      ?? throw NotFoundException.For("Brigada", id);
        brigada.Status = status;
        _brigadas.Update(brigada);
        await _brigadas.SaveChangesAsync(ct);
        return brigada.ToResponse();
    }
}
