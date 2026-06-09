using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Sentinela.Tests.Integration;

/// <summary>
/// Testes de integracao da API de Leituras, validando a camada de validacao de modelo
/// (DataAnnotations -> ProblemDetails 400) atraves do pipeline HTTP real.
/// </summary>
public class LeiturasApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LeiturasApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task PostLeitura_CorpoInvalido_DeveRetornar400()
    {
        // Arrange: faltam TipoMedida e Valor (campos obrigatorios).
        var client = _factory.CreateClient();
        var corpoInvalido = new { sensorId = 1, unidade = "C" };

        // Act
        var response = await client.PostAsJsonAsync("/api/leituras", corpoInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
