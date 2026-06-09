using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Medicoes IoT enviadas pelos sensores (Parte 6 alimenta este recurso).</summary>
[ApiController]
[Route("api/leituras")]
[Produces("application/json")]
[Tags("Leituras (IoT)")]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _service;

    public LeiturasController(ILeituraService service) => _service = service;

    /// <summary>Lista leituras, com filtro opcional por sensor.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LeituraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LeituraResponse>>> GetAll([FromQuery] int? sensorId, CancellationToken ct)
        => Ok(await _service.GetAllAsync(sensorId, ct));

    /// <summary>Obtem uma leitura pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LeituraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Registra uma nova leitura de sensor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeituraResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LeituraResponse>> Create([FromBody] LeituraCreateRequest request, CancellationToken ct)
    {
        var criada = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }
}
