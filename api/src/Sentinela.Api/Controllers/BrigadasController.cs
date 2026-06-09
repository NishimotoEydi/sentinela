using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Equipes de combate a incendio.</summary>
[ApiController]
[Route("api/brigadas")]
[Produces("application/json")]
[Tags("Brigadas")]
public class BrigadasController : ControllerBase
{
    private readonly IBrigadaService _service;

    public BrigadasController(IBrigadaService service) => _service = service;

    /// <summary>Lista brigadas, com filtro opcional por status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BrigadaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BrigadaResponse>>> GetAll([FromQuery] StatusBrigada? status, CancellationToken ct)
        => Ok(await _service.GetAllAsync(status, ct));

    /// <summary>Obtem uma brigada pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BrigadaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrigadaResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Cadastra uma nova brigada.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BrigadaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrigadaResponse>> Create([FromBody] BrigadaCreateRequest request, CancellationToken ct)
    {
        var criada = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    /// <summary>Altera o status de disponibilidade da brigada.</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(BrigadaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrigadaResponse>> UpdateStatus(int id, [FromBody] BrigadaStatusUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateStatusAsync(id, request.Status!.Value, ct));
}
