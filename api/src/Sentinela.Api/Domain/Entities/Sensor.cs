using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>Estacao IoT fisica instalada em campo. Gera muitas <see cref="Leitura"/>.</summary>
public class Sensor
{
    public int Id { get; set; }
    public int AreaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public Conectividade Conectividade { get; set; } = Conectividade.Lora;
    public FonteEnergia FonteEnergia { get; set; } = FonteEnergia.Solar;
    public StatusSensor Status { get; set; } = StatusSensor.Ativo;
    public DateTime DataInstalacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public AreaMonitorada? Area { get; set; }
    public ICollection<Leitura> Leituras { get; set; } = new List<Leitura>();
}
