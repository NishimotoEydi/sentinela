using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Despachos de brigadas para alertas (resolve o N:N alerta x brigada).</summary>
[ApiController]
[Route("api/atendimentos")]
[Produces("application/json")]
[Tags("Atendimentos (Despacho)")]
public class AtendimentosController : ControllerBase
{
    private readonly IAtendimentoService _service;

    public AtendimentosController(IAtendimentoService service) => _service = service;

    /// <summary>Lista atendimentos, com filtro opcional por alerta.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AtendimentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AtendimentoResponse>>> GetAll([FromQuery] int? alertaId, CancellationToken ct)
        => Ok(await _service.GetAllAsync(alertaId, ct));

    /// <summary>Obtem um atendimento pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AtendimentoResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Despacha uma brigada para um alerta.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AtendimentoResponse>> Despachar([FromBody] AtendimentoCreateRequest request, CancellationToken ct)
    {
        var criado = await _service.DespacharAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza o status do atendimento (despachado -> em combate -> concluido).</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AtendimentoResponse>> UpdateStatus(int id, [FromBody] AtendimentoStatusUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateStatusAsync(id, request.Status!.Value, ct));
}
