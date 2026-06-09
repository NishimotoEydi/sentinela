using System.ComponentModel.DataAnnotations;

namespace Sentinela.Api.Dtos;

/// <summary>Representacao de saida de uma area monitorada.</summary>
public record AreaResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Bioma { get; init; } = string.Empty;
    public string Municipio { get; init; } = string.Empty;
    public string Uf { get; init; } = string.Empty;
    public decimal LatitudeCentro { get; init; }
    public decimal LongitudeCentro { get; init; }
    public decimal? AreaHectares { get; init; }
    public decimal? BboxNorte { get; init; }
    public decimal? BboxSul { get; init; }
    public decimal? BboxLeste { get; init; }
    public decimal? BboxOeste { get; init; }
    public DateTime DataCadastro { get; init; }
}

/// <summary>Dados para cadastrar/atualizar uma area monitorada.</summary>
public record AreaCreateRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Nome { get; init; } = string.Empty;

    [StringLength(40)]
    public string Bioma { get; init; } = "Pantanal";

    [Required, StringLength(80)]
    public string Municipio { get; init; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string Uf { get; init; } = string.Empty;

    [Range(-90, 90)]
    public decimal LatitudeCentro { get; init; }

    [Range(-180, 180)]
    public decimal LongitudeCentro { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? AreaHectares { get; init; }

    public decimal? BboxNorte { get; init; }
    public decimal? BboxSul { get; init; }
    public decimal? BboxLeste { get; init; }
    public decimal? BboxOeste { get; init; }
}
