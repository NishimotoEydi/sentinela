using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Domain.Entities;

/// <summary>
/// Despacho de uma <see cref="Brigada"/> para um <see cref="Alerta"/>.
/// Resolve o N:N entre alerta e brigada e registra o ciclo do despacho.
/// </summary>
public class Atendimento
{
    public int Id { get; set; }
    public int AlertaId { get; set; }
    public int BrigadaId { get; set; }
    public StatusAtendimento Status { get; set; } = StatusAtendimento.Despachado;
    public DateTime DataHoraDespacho { get; set; } = DateTime.UtcNow;
    public DateTime? DataHoraChegada { get; set; }
    public DateTime? DataHoraConclusao { get; set; }
    public string? Observacoes { get; set; }

    // Relacionamentos
    public Alerta? Alerta { get; set; }
    public Brigada? Brigada { get; set; }
}
