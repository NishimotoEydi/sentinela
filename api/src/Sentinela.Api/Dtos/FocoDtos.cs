using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record FocoResponse
{
    public int Id { get; init; }
    public int AreaId { get; init; }
    public FonteFoco Fonte { get; init; }
    public string? Satelite { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public decimal? TemperaturaBrilho { get; init; }
    public decimal? Frp { get; init; }
    public int? Confianca { get; init; }
    public DateTime DataHoraDeteccao { get; init; }
}

/// <summary>Foco de calor, tipicamente ingerido do NASA FIRMS (satelite).</summary>
public record FocoCreateRequest
{
    [Required, Range(1, int.MaxValue)]
    public int AreaId { get; init; }

    [Required]
    public FonteFoco? Fonte { get; init; }

    [StringLength(20)]
    public string? Satelite { get; init; }

    [Range(-90, 90)]
    public decimal Latitude { get; init; }

    [Range(-180, 180)]
    public decimal Longitude { get; init; }

    public decimal? TemperaturaBrilho { get; init; }
    public decimal? Frp { get; init; }

    [Range(0, 100)]
    public int? Confianca { get; init; }

    public DateTime? DataHoraDeteccao { get; init; }
}
