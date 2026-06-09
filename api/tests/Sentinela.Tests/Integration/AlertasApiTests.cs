using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Sentinela.Tests.Integration;

/// <summary>
/// Testes de integracao da API de Alertas via <see cref="WebApplicationFactory{TEntryPoint}"/>,
/// exercitando o pipeline HTTP completo (controller -> service -> repositorio -> InMemory + seed).
/// Focam em status code e presenca de dados, sem depender de contagem global exata.
/// </summary>
public class AlertasApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AlertasApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task GetAlertas_FiltrandoPorNivelCritico_DeveRetornar200ComPeloMenosUm()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/alertas?nivel=Critico");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var corpo = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(corpo);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.GetArrayLength() >= 1, "Esperava ao menos 1 alerta critico do seed.");
        // O alerta critico do seed e de origem Fusao; enums serializam como string.
        Assert.Contains("Critico", corpo);
        Assert.Contains("Fusao", corpo);
    }

    [Fact]
    public async Task GetAlertaPorId_Inexistente_DeveRetornar404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/alertas/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
