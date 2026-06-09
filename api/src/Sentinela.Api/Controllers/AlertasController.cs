using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Alertas gerados pelas deteccoes de satelite e de sensores.</summary>
[ApiController]
[Route("api/alertas")]
[Produces("application/json")]
[Tags("Alertas")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _service;

    public AlertasController(IAlertaService service) => _service = service;

    /// <summary>Lista alertas, com filtros opcionais por nivel, status e area.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlertaResponse>>> GetAll(
        [FromQuery] NivelAlerta? nivel, [FromQuery] StatusAlerta? status, [FromQuery] int? areaId, CancellationToken ct)
        => Ok(await _service.GetAllAsync(nivel, status, areaId, ct));

    /// <summary>Obtem um alerta pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Abre um novo alerta manualmente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AlertaResponse>> Create([FromBody] AlertaCreateRequest request, CancellationToken ct)
    {
        var criado = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza o status do alerta (ex.: Aberto -> EmAtendimento -> Resolvido).</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaResponse>> UpdateStatus(int id, [FromBody] AlertaStatusUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateStatusAsync(id, request.Status!.Value, ct));

    /// <summary>Exclui um alerta.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
