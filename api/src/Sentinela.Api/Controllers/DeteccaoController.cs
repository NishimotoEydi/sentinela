using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Dtos;
using Sentinela.Api.Services;

namespace Sentinela.Api.Controllers;

/// <summary>
/// Avaliacao de deteccao por sensor. Analisa as leituras recentes e, havendo
/// indicio de fogo, abre um alerta (CRITICO quando ha confirmacao de satelite).
/// </summary>
[ApiController]
[Route("api/deteccao")]
[Produces("application/json")]
[Tags("Deteccao (Motor de Fusao)")]
public class DeteccaoController : ControllerBase
{
    private readonly IDeteccaoService _service;

    public DeteccaoController(IDeteccaoService service) => _service = service;

    /// <summary>Avalia o sensor informado e gera alerta automatico se detectar fogo.</summary>
    [HttpPost("avaliar/{sensorId:int}")]
    [ProducesResponseType(typeof(DeteccaoResultado), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeteccaoResultado>> Avaliar(int sensorId, CancellationToken ct)
        => Ok(await _service.AvaliarSensorAsync(sensorId, ct));
}
