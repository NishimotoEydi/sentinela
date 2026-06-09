using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface ILeituraService
{
    Task<IReadOnlyList<LeituraResponse>> GetAllAsync(int? sensorId, CancellationToken ct = default);
    Task<LeituraResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LeituraResponse> CreateAsync(LeituraCreateRequest request, CancellationToken ct = default);
}

public class LeituraService : ILeituraService
{
    private readonly IRepository<Leitura> _leituras;
    private readonly IRepository<Sensor> _sensores;

    public LeituraService(IRepository<Leitura> leituras, IRepository<Sensor> sensores)
    {
        _leituras = leituras;
        _sensores = sensores;
    }

    public async Task<IReadOnlyList<LeituraResponse>> GetAllAsync(int? sensorId, CancellationToken ct = default)
    {
        var query = _leituras.Query().AsNoTracking();
        if (sensorId is not null)
        {
            query = query.Where(l => l.SensorId == sensorId);
        }

        var lista = await query.OrderByDescending(l => l.DataHora).ToListAsync(ct);
        return lista.Select(l => l.ToResponse()).ToList();
    }

    public async Task<LeituraResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var leitura = await _leituras.GetByIdAsync(id, ct)
                      ?? throw NotFoundException.For("Leitura", id);
        return leitura.ToResponse();
    }

    public async Task<LeituraResponse> CreateAsync(LeituraCreateRequest request, CancellationToken ct = default)
    {
        if (!await _sensores.ExistsAsync(request.SensorId, ct))
        {
            throw new BusinessRuleException($"Sensor com id {request.SensorId} nao existe.");
        }

        var leitura = request.ToEntity();
        await _leituras.AddAsync(leitura, ct);
        await _leituras.SaveChangesAsync(ct);
        return leitura.ToResponse();
    }
}
