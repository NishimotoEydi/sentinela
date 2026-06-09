using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IAreaService
{
    Task<IReadOnlyList<AreaResponse>> GetAllAsync(CancellationToken ct = default);
    Task<AreaResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AreaResponse> CreateAsync(AreaCreateRequest request, CancellationToken ct = default);
    Task<AreaResponse> UpdateAsync(int id, AreaCreateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public class AreaService : IAreaService
{
    private readonly IRepository<AreaMonitorada> _areas;
    private readonly IRepository<Sensor> _sensores;
    private readonly IRepository<Alerta> _alertas;

    public AreaService(
        IRepository<AreaMonitorada> areas,
        IRepository<Sensor> sensores,
        IRepository<Alerta> alertas)
    {
        _areas = areas;
        _sensores = sensores;
        _alertas = alertas;
    }

    public async Task<IReadOnlyList<AreaResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var lista = await _areas.Query().AsNoTracking().OrderBy(a => a.Nome).ToListAsync(ct);
        return lista.Select(a => a.ToResponse()).ToList();
    }

    public async Task<AreaResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var area = await _areas.GetByIdAsync(id, ct)
                   ?? throw NotFoundException.For("Area", id);
        return area.ToResponse();
    }

    public async Task<AreaResponse> CreateAsync(AreaCreateRequest request, CancellationToken ct = default)
    {
        var area = request.ToEntity();
        area.DataCadastro = DateTime.UtcNow;
        await _areas.AddAsync(area, ct);
        await _areas.SaveChangesAsync(ct);
        return area.ToResponse();
    }

    public async Task<AreaResponse> UpdateAsync(int id, AreaCreateRequest request, CancellationToken ct = default)
    {
        var area = await _areas.GetByIdAsync(id, ct)
                   ?? throw NotFoundException.For("Area", id);
        request.ApplyTo(area);
        _areas.Update(area);
        await _areas.SaveChangesAsync(ct);
        return area.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var area = await _areas.GetByIdAsync(id, ct)
                   ?? throw NotFoundException.For("Area", id);

        var temSensores = await _sensores.Query().AnyAsync(s => s.AreaId == id, ct);
        var temAlertas = await _alertas.Query().AnyAsync(a => a.AreaId == id, ct);
        if (temSensores || temAlertas)
        {
            throw new BusinessRuleException(
                "Nao e possivel excluir a area: existem sensores ou alertas vinculados a ela.");
        }

        _areas.Remove(area);
        await _areas.SaveChangesAsync(ct);
    }
}
