using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface ISensorService
{
    Task<IReadOnlyList<SensorResponse>> GetAllAsync(int? areaId, StatusSensor? status, CancellationToken ct = default);
    Task<SensorResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SensorResponse> CreateAsync(SensorCreateRequest request, CancellationToken ct = default);
    Task<SensorResponse> UpdateAsync(int id, SensorUpdateRequest request, CancellationToken ct = default);
    Task<SensorResponse> UpdateStatusAsync(int id, StatusSensor status, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public class SensorService : ISensorService
{
    private readonly IRepository<Sensor> _sensores;
    private readonly IRepository<AreaMonitorada> _areas;

    public SensorService(IRepository<Sensor> sensores, IRepository<AreaMonitorada> areas)
    {
        _sensores = sensores;
        _areas = areas;
    }

    public async Task<IReadOnlyList<SensorResponse>> GetAllAsync(int? areaId, StatusSensor? status, CancellationToken ct = default)
    {
        var query = _sensores.Query().AsNoTracking();
        if (areaId is not null)
        {
            query = query.Where(s => s.AreaId == areaId);
        }
        if (status is not null)
        {
            query = query.Where(s => s.Status == status);
        }

        var lista = await query.OrderBy(s => s.Codigo).ToListAsync(ct);
        return lista.Select(s => s.ToResponse()).ToList();
    }

    public async Task<SensorResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sensor = await _sensores.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Sensor", id);
        return sensor.ToResponse();
    }

    public async Task<SensorResponse> CreateAsync(SensorCreateRequest request, CancellationToken ct = default)
    {
        if (!await _areas.ExistsAsync(request.AreaId, ct))
        {
            throw new BusinessRuleException($"Area com id {request.AreaId} nao existe.");
        }

        var codigoEmUso = await _sensores.Query().AnyAsync(s => s.Codigo == request.Codigo, ct);
        if (codigoEmUso)
        {
            throw new BusinessRuleException($"Ja existe um sensor com o codigo '{request.Codigo}'.");
        }

        var sensor = request.ToEntity();
        sensor.DataInstalacao = DateTime.UtcNow;
        await _sensores.AddAsync(sensor, ct);
        await _sensores.SaveChangesAsync(ct);
        return sensor.ToResponse();
    }

    public async Task<SensorResponse> UpdateAsync(int id, SensorUpdateRequest request, CancellationToken ct = default)
    {
        var sensor = await _sensores.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Sensor", id);

        var codigoEmUso = await _sensores.Query()
            .AnyAsync(s => s.Codigo == request.Codigo && s.Id != id, ct);
        if (codigoEmUso)
        {
            throw new BusinessRuleException($"Ja existe outro sensor com o codigo '{request.Codigo}'.");
        }

        request.ApplyTo(sensor);
        _sensores.Update(sensor);
        await _sensores.SaveChangesAsync(ct);
        return sensor.ToResponse();
    }

    public async Task<SensorResponse> UpdateStatusAsync(int id, StatusSensor status, CancellationToken ct = default)
    {
        var sensor = await _sensores.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Sensor", id);
        sensor.Status = status;
        _sensores.Update(sensor);
        await _sensores.SaveChangesAsync(ct);
        return sensor.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var sensor = await _sensores.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Sensor", id);
        _sensores.Remove(sensor);
        await _sensores.SaveChangesAsync(ct);
    }
}
