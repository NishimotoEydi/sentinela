using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Territorios sob vigilancia.</summary>
[ApiController]
[Route("api/areas")]
[Produces("application/json")]
[Tags("Areas Monitoradas")]
public class AreasController : ControllerBase
{
    private readonly IAreaService _service;

    public AreasController(IAreaService service) => _service = service;

    /// <summary>Lista todas as areas monitoradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AreaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AreaResponse>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    /// <summary>Obtem uma area pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AreaResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Cadastra uma nova area monitorada.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AreaResponse>> Create([FromBody] AreaCreateRequest request, CancellationToken ct)
    {
        var criada = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza uma area existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AreaResponse>> Update(int id, [FromBody] AreaCreateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, request, ct));

    /// <summary>Exclui uma area (bloqueado se houver sensores ou alertas vinculados).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
