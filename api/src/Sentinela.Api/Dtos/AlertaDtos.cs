using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record AlertaResponse
{
    public int Id { get; init; }
    public int AreaId { get; init; }
    public int? FocoId { get; init; }
    public NivelAlerta Nivel { get; init; }
    public OrigemAlerta Origem { get; init; }
    public StatusAlerta Status { get; init; }
    public string? Descricao { get; init; }
    public DateTime DataHoraAbertura { get; init; }
    public DateTime? DataHoraResolucao { get; init; }
}

public record AlertaCreateRequest
{
    [Required, Range(1, int.MaxValue)]
    public int AreaId { get; init; }

    public int? FocoId { get; init; }

    [Required]
    public NivelAlerta? Nivel { get; init; }

    [Required]
    public OrigemAlerta? Origem { get; init; }

    [StringLength(300)]
    public string? Descricao { get; init; }
}

public record AlertaStatusUpdateRequest
{
    [Required]
    public StatusAlerta? Status { get; init; }
}
