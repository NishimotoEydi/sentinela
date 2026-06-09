using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>Focos de calor (hotspots de satelite NASA FIRMS ou derivados do solo).</summary>
[ApiController]
[Route("api/focos")]
[Produces("application/json")]
[Tags("Focos de Calor (Satelite)")]
public class FocosController : ControllerBase
{
    private readonly IFocoService _service;

    public FocosController(IFocoService service) => _service = service;

    /// <summary>Lista focos, com filtros opcionais por area e por data inicial.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FocoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FocoResponse>>> GetAll(
        [FromQuery] int? areaId, [FromQuery] DateTime? desde, CancellationToken ct)
        => Ok(await _service.GetAllAsync(areaId, desde, ct));

    /// <summary>Obtem um foco pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FocoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FocoResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Registra um novo foco de calor (ex.: ingestao de hotspot do FIRMS).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FocoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FocoResponse>> Create([FromBody] FocoCreateRequest request, CancellationToken ct)
    {
        var criado = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }
}
