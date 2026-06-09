using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record SensorResponse
{
    public int Id { get; init; }
    public int AreaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public Conectividade Conectividade { get; init; }
    public FonteEnergia FonteEnergia { get; init; }
    public StatusSensor Status { get; init; }
    public DateTime DataInstalacao { get; init; }
}

public record SensorCreateRequest
{
    [Required(ErrorMessage = "AreaId e obrigatorio."), Range(1, int.MaxValue)]
    public int AreaId { get; init; }

    [Required, StringLength(30, MinimumLength = 2)]
    public string Codigo { get; init; } = string.Empty;

    [Range(-90, 90)]
    public decimal Latitude { get; init; }

    [Range(-180, 180)]
    public decimal Longitude { get; init; }

    public Conectividade Conectividade { get; init; } = Conectividade.Lora;
    public FonteEnergia FonteEnergia { get; init; } = FonteEnergia.Solar;
    public StatusSensor Status { get; init; } = StatusSensor.Ativo;
}

public record SensorUpdateRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    public string Codigo { get; init; } = string.Empty;

    [Range(-90, 90)]
    public decimal Latitude { get; init; }

    [Range(-180, 180)]
    public decimal Longitude { get; init; }

    public Conectividade Conectividade { get; init; } = Conectividade.Lora;
    public FonteEnergia FonteEnergia { get; init; } = FonteEnergia.Solar;
    public StatusSensor Status { get; init; } = StatusSensor.Ativo;
}

public record SensorStatusUpdateRequest
{
    [Required]
    public StatusSensor? Status { get; init; }
}
