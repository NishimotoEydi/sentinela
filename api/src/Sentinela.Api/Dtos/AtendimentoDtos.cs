using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record AtendimentoResponse
{
    public int Id { get; init; }
    public int AlertaId { get; init; }
    public int BrigadaId { get; init; }
    public StatusAtendimento Status { get; init; }
    public DateTime DataHoraDespacho { get; init; }
    public DateTime? DataHoraChegada { get; init; }
    public DateTime? DataHoraConclusao { get; init; }
    public string? Observacoes { get; init; }
}

/// <summary>Despacha uma brigada para um alerta.</summary>
public record AtendimentoCreateRequest
{
    [Required, Range(1, int.MaxValue)]
    public int AlertaId { get; init; }

    [Required, Range(1, int.MaxValue)]
    public int BrigadaId { get; init; }

    [StringLength(300)]
    public string? Observacoes { get; init; }
}

public record AtendimentoStatusUpdateRequest
{
    [Required]
    public StatusAtendimento? Status { get; init; }
}
