using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IFocoService
{
    Task<IReadOnlyList<FocoResponse>> GetAllAsync(int? areaId, DateTime? desde, CancellationToken ct = default);
    Task<FocoResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<FocoResponse> CreateAsync(FocoCreateRequest request, CancellationToken ct = default);
}

public class FocoService : IFocoService
{
    private readonly IRepository<FocoCalor> _focos;
    private readonly IRepository<AreaMonitorada> _areas;

    public FocoService(IRepository<FocoCalor> focos, IRepository<AreaMonitorada> areas)
    {
        _focos = focos;
        _areas = areas;
    }

    public async Task<IReadOnlyList<FocoResponse>> GetAllAsync(int? areaId, DateTime? desde, CancellationToken ct = default)
    {
        var query = _focos.Query().AsNoTracking();
        if (areaId is not null)
        {
            query = query.Where(f => f.AreaId == areaId);
        }
        if (desde is not null)
        {
            query = query.Where(f => f.DataHoraDeteccao >= desde);
        }

        var lista = await query.OrderByDescending(f => f.DataHoraDeteccao).ToListAsync(ct);
        return lista.Select(f => f.ToResponse()).ToList();
    }

    public async Task<FocoResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var foco = await _focos.GetByIdAsync(id, ct)
                   ?? throw NotFoundException.For("Foco de calor", id);
        return foco.ToResponse();
    }

    public async Task<FocoResponse> CreateAsync(FocoCreateRequest request, CancellationToken ct = default)
    {
        if (!await _areas.ExistsAsync(request.AreaId, ct))
        {
            throw new BusinessRuleException($"Area com id {request.AreaId} nao existe.");
        }

        var foco = request.ToEntity();
        await _focos.AddAsync(foco, ct);
        await _focos.SaveChangesAsync(ct);
        return foco.ToResponse();
    }
}
