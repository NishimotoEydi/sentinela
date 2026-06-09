using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>
/// Alerta gerado a partir das deteccoes. O campo <see cref="Origem"/> indica como
/// o alerta foi criado; quando satelite e solo concordam (Fusao) o nivel sobe.
/// </summary>
public class Alerta
{
    public int Id { get; set; }
    public int AreaId { get; set; }

    /// <summary>Foco que originou o alerta (opcional).</summary>
    public int? FocoId { get; set; }

    public NivelAlerta Nivel { get; set; }
    public OrigemAlerta Origem { get; set; }
    public StatusAlerta Status { get; set; } = StatusAlerta.Aberto;
    public string? Descricao { get; set; }
    public DateTime DataHoraAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataHoraResolucao { get; set; }

    // Relacionamentos
    public AreaMonitorada? Area { get; set; }
    public FocoCalor? Foco { get; set; }
    public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}
