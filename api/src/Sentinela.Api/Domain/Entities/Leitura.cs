using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>Medicao enviada por um <see cref="Sensor"/> (dado IoT bruto).</summary>
public class Leitura
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public TipoMedida TipoMedida { get; set; }
    public decimal Valor { get; set; }
    public string Unidade { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Sensor? Sensor { get; set; }
}
