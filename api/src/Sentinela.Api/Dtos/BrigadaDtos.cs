using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record BrigadaResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoBrigada Tipo { get; init; }
    public decimal BaseLatitude { get; init; }
    public decimal BaseLongitude { get; init; }
    public string? Contato { get; init; }
    public int? Efetivo { get; init; }
    public StatusBrigada Status { get; init; }
}

public record BrigadaCreateRequest
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Nome { get; init; } = string.Empty;

    [Required]
    public TipoBrigada? Tipo { get; init; }

    [Range(-90, 90)]
    public decimal BaseLatitude { get; init; }

    [Range(-180, 180)]
    public decimal BaseLongitude { get; init; }

    [StringLength(20)]
    public string? Contato { get; init; }

    [Range(0, 1000)]
    public int? Efetivo { get; init; }

    public StatusBrigada Status { get; init; } = StatusBrigada.Disponivel;
}

public record BrigadaStatusUpdateRequest
{
    [Required]
    public StatusBrigada? Status { get; init; }
}
