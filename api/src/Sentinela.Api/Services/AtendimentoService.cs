using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Mapping;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IAtendimentoService
{
    Task<IReadOnlyList<AtendimentoResponse>> GetAllAsync(int? alertaId, CancellationToken ct = default);
    Task<AtendimentoResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AtendimentoResponse> DespacharAsync(AtendimentoCreateRequest request, CancellationToken ct = default);
    Task<AtendimentoResponse> UpdateStatusAsync(int id, StatusAtendimento status, CancellationToken ct = default);
}

public class AtendimentoService : IAtendimentoService
{
    private readonly IRepository<Atendimento> _atendimentos;
    private readonly IRepository<Alerta> _alertas;
    private readonly IRepository<Brigada> _brigadas;

    public AtendimentoService(
        IRepository<Atendimento> atendimentos,
        IRepository<Alerta> alertas,
        IRepository<Brigada> brigadas)
    {
        _atendimentos = atendimentos;
        _alertas = alertas;
        _brigadas = brigadas;
    }

    public async Task<IReadOnlyList<AtendimentoResponse>> GetAllAsync(int? alertaId, CancellationToken ct = default)
    {
        var query = _atendimentos.Query().AsNoTracking();
        if (alertaId is not null)
        {
            query = query.Where(a => a.AlertaId == alertaId);
        }

        var lista = await query.OrderByDescending(a => a.DataHoraDespacho).ToListAsync(ct);
        return lista.Select(a => a.ToResponse()).ToList();
    }

    public async Task<AtendimentoResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var atendimento = await _atendimentos.GetByIdAsync(id, ct)
                          ?? throw NotFoundException.For("Atendimento", id);
        return atendimento.ToResponse();
    }

    public async Task<AtendimentoResponse> DespacharAsync(AtendimentoCreateRequest request, CancellationToken ct = default)
    {
        var alerta = await _alertas.GetByIdAsync(request.AlertaId, ct)
                     ?? throw new BusinessRuleException($"Alerta com id {request.AlertaId} nao existe.");
        var brigada = await _brigadas.GetByIdAsync(request.BrigadaId, ct)
                      ?? throw new BusinessRuleException($"Brigada com id {request.BrigadaId} nao existe.");

        if (brigada.Status == StatusBrigada.Indisponivel)
        {
            throw new BusinessRuleException(
                $"A brigada '{brigada.Nome}' esta indisponivel e nao pode ser despachada.");
        }

        var atendimento = request.ToEntity();
        atendimento.Status = StatusAtendimento.Despachado;
        atendimento.DataHoraDespacho = DateTime.UtcNow;
        await _atendimentos.AddAsync(atendimento, ct);

        // Efeitos colaterais do despacho: brigada entra em operacao e alerta passa a em atendimento.
        brigada.Status = StatusBrigada.EmOperacao;
        _brigadas.Update(brigada);
        if (alerta.Status == StatusAlerta.Aberto)
        {
            alerta.Status = StatusAlerta.EmAtendimento;
            _alertas.Update(alerta);
        }

        await _atendimentos.SaveChangesAsync(ct);
        return atendimento.ToResponse();
    }

    public async Task<AtendimentoResponse> UpdateStatusAsync(int id, StatusAtendimento status, CancellationToken ct = default)
    {
        var atendimento = await _atendimentos.GetByIdAsync(id, ct)
                          ?? throw NotFoundException.For("Atendimento", id);

        atendimento.Status = status;
        switch (status)
        {
            case StatusAtendimento.EmCombate when atendimento.DataHoraChegada is null:
                atendimento.DataHoraChegada = DateTime.UtcNow;
                break;
            case StatusAtendimento.Concluido:
                atendimento.DataHoraConclusao = DateTime.UtcNow;
                // Libera a brigada de volta para disponivel ao concluir.
                var brigada = await _brigadas.GetByIdAsync(atendimento.BrigadaId, ct);
                if (brigada is not null)
                {
                    brigada.Status = StatusBrigada.Disponivel;
                    _brigadas.Update(brigada);
                }
                break;
        }

        _atendimentos.Update(atendimento);
        await _atendimentos.SaveChangesAsync(ct);
        return atendimento.ToResponse();
    }
}
