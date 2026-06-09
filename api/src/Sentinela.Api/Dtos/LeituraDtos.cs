using System.ComponentModel.DataAnnotations;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

public record LeituraResponse
{
    public int Id { get; init; }
    public int SensorId { get; init; }
    public TipoMedida TipoMedida { get; init; }
    public decimal Valor { get; init; }
    public string Unidade { get; init; } = string.Empty;
    public DateTime DataHora { get; init; }
}

/// <summary>Medicao recebida de um sensor IoT (Parte 6 alimenta este endpoint).</summary>
public record LeituraCreateRequest
{
    [Required(ErrorMessage = "SensorId e obrigatorio."), Range(1, int.MaxValue)]
    public int SensorId { get; init; }

    [Required(ErrorMessage = "TipoMedida e obrigatorio (Temperatura, Umidade ou Fumaca).")]
    public TipoMedida? TipoMedida { get; init; }

    [Required]
    [Range(-100, 100000, ErrorMessage = "Valor fora da faixa plausivel.")]
    public decimal Valor { get; init; }

    [Required, StringLength(10)]
    public string Unidade { get; init; } = string.Empty;

    /// <summary>Opcional. Se omitido, usa o horario do servidor.</summary>
    public DateTime? DataHora { get; init; }
}
