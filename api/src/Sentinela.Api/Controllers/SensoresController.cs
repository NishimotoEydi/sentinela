using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Estacoes IoT instaladas em campo.</summary>
[ApiController]
[Route("api/sensores")]
[Produces("application/json")]
[Tags("Sensores")]
public class SensoresController : ControllerBase
{
    private readonly ISensorService _service;

    public SensoresController(ISensorService service) => _service = service;

    /// <summary>Lista sensores, com filtros opcionais por area e por status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SensorResponse>>> GetAll(
        [FromQuery] int? areaId, [FromQuery] StatusSensor? status, CancellationToken ct)
        => Ok(await _service.GetAllAsync(areaId, status, ct));

    /// <summary>Obtem um sensor pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Cadastra um novo sensor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SensorResponse>> Create([FromBody] SensorCreateRequest request, CancellationToken ct)
    {
        var criado = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um sensor existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> Update(int id, [FromBody] SensorUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, request, ct));

    /// <summary>Altera apenas o status operacional do sensor.</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> UpdateStatus(int id, [FromBody] SensorStatusUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateStatusAsync(id, request.Status!.Value, ct));

    /// <summary>Exclui um sensor.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
