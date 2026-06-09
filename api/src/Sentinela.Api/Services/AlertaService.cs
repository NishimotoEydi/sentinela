using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaResponse>> GetAllAsync(NivelAlerta? nivel, StatusAlerta? status, int? areaId, CancellationToken ct = default);
    Task<AlertaResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AlertaResponse> CreateAsync(AlertaCreateRequest request, CancellationToken ct = default);
    Task<AlertaResponse> UpdateStatusAsync(int id, StatusAlerta status, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public class AlertaService : IAlertaService
{
    private readonly IRepository<Alerta> _alertas;
    private readonly IRepository<AreaMonitorada> _areas;
    private readonly IRepository<FocoCalor> _focos;

    public AlertaService(
        IRepository<Alerta> alertas,
        IRepository<AreaMonitorada> areas,
        IRepository<FocoCalor> focos)
    {
        _alertas = alertas;
        _areas = areas;
        _focos = focos;
    }

    public async Task<IReadOnlyList<AlertaResponse>> GetAllAsync(NivelAlerta? nivel, StatusAlerta? status, int? areaId, CancellationToken ct = default)
    {
        var query = _alertas.Query().AsNoTracking();
        if (nivel is not null)
        {
            query = query.Where(a => a.Nivel == nivel);
        }
        if (status is not null)
        {
            query = query.Where(a => a.Status == status);
        }
        if (areaId is not null)
        {
            query = query.Where(a => a.AreaId == areaId);
        }

        var lista = await query.OrderByDescending(a => a.DataHoraAbertura).ToListAsync(ct);
        return lista.Select(a => a.ToResponse()).ToList();
    }

    public async Task<AlertaResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var alerta = await _alertas.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Alerta", id);
        return alerta.ToResponse();
    }

    public async Task<AlertaResponse> CreateAsync(AlertaCreateRequest request, CancellationToken ct = default)
    {
        if (!await _areas.ExistsAsync(request.AreaId, ct))
        {
            throw new BusinessRuleException($"Area com id {request.AreaId} nao existe.");
        }
        if (request.FocoId is not null && !await _focos.ExistsAsync(request.FocoId.Value, ct))
        {
            throw new BusinessRuleException($"Foco de calor com id {request.FocoId} nao existe.");
        }

        var alerta = request.ToEntity();
        alerta.Status = StatusAlerta.Aberto;
        alerta.DataHoraAbertura = DateTime.UtcNow;
        await _alertas.AddAsync(alerta, ct);
        await _alertas.SaveChangesAsync(ct);
        return alerta.ToResponse();
    }

    public async Task<AlertaResponse> UpdateStatusAsync(int id, StatusAlerta status, CancellationToken ct = default)
    {
        var alerta = await _alertas.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Alerta", id);

        alerta.Status = status;
        // Ao resolver (ou marcar falso-positivo), registra o horario de resolucao.
        if (status is StatusAlerta.Resolvido or StatusAlerta.FalsoPositivo)
        {
            alerta.DataHoraResolucao = DateTime.UtcNow;
        }
        else
        {
            alerta.DataHoraResolucao = null;
        }

        _alertas.Update(alerta);
        await _alertas.SaveChangesAsync(ct);
        return alerta.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var alerta = await _alertas.GetByIdAsync(id, ct)
                     ?? throw NotFoundException.For("Alerta", id);
        _alertas.Remove(alerta);
        await _alertas.SaveChangesAsync(ct);
    }
}
