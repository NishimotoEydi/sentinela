using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>Equipe de combate a incendio acionada nos atendimentos.</summary>
public class Brigada
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoBrigada Tipo { get; set; }
    public decimal BaseLatitude { get; set; }
    public decimal BaseLongitude { get; set; }
    public string? Contato { get; set; }
    public int? Efetivo { get; set; }
    public StatusBrigada Status { get; set; } = StatusBrigada.Disponivel;

    // Relacionamentos
    public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}
