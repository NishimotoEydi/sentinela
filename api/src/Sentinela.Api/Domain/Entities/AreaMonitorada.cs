namespace Sentinela.Api.Domain.Entities;

/// <summary>
/// Territorio sob vigilancia (um parque ou setor de municipio). Sensores, focos de
/// calor e alertas referenciam uma area. O bounding box (bbox_*) e o retangulo usado
/// para consultar a API do NASA FIRMS na regiao.
/// </summary>
public class AreaMonitorada
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Bioma { get; set; } = "Pantanal";
    public string Municipio { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public decimal LatitudeCentro { get; set; }
    public decimal LongitudeCentro { get; set; }
    public decimal? AreaHectares { get; set; }
    public decimal? BboxNorte { get; set; }
    public decimal? BboxSul { get; set; }
    public decimal? BboxLeste { get; set; }
    public decimal? BboxOeste { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Sensor> Sensores { get; set; } = new List<Sensor>();
    public ICollection<FocoCalor> FocosCalor { get; set; } = new List<FocoCalor>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}
