using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>
/// Foco de calor detectado. Pode vir do satelite (campos temperatura de brilho,
/// FRP e confianca sao os campos reais do NASA FIRMS) ou ser derivado dos sensores
/// de solo. Foco ainda nao e alerta: e apenas "detectei calor aqui".
/// </summary>
public class FocoCalor
{
    public int Id { get; set; }
    public int AreaId { get; set; }
    public FonteFoco Fonte { get; set; }
    public string? Satelite { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    /// <summary>Temperatura de brilho em Kelvin (bright_ti4/ti5 do FIRMS).</summary>
    public decimal? TemperaturaBrilho { get; set; }

    /// <summary>Fire Radiative Power, em MW.</summary>
    public decimal? Frp { get; set; }

    /// <summary>Confianca da deteccao (0..100).</summary>
    public int? Confianca { get; set; }

    public DateTime DataHoraDeteccao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public AreaMonitorada? Area { get; set; }
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}
