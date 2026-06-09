using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Dtos;

/// <summary>
/// Resultado da avaliacao de um sensor: indica se o solo detectou fogo, se houve
/// confirmacao de satelite e qual alerta foi gerado.
/// </summary>
public record DeteccaoResultado
{
    public int SensorId { get; init; }
    public bool FogoDetectadoSolo { get; init; }
    public bool ConfirmadoPorSatelite { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public int? FocoSoloId { get; init; }
    public int? AlertaId { get; init; }
    public NivelAlerta? Nivel { get; init; }
    public OrigemAlerta? Origem { get; init; }
}
