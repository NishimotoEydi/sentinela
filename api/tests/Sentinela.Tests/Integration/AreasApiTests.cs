using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Sentinela.Tests.Integration;

/// <summary>
/// Testes de integracao da API de Areas Monitoradas, cobrindo a criacao
/// bem-sucedida (201 + Location) pelo pipeline HTTP real.
/// </summary>
public class AreasApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AreasApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task PostArea_Valida_DeveRetornar201ComLocationEId()
    {
        // Arrange
        var client = _factory.CreateClient();
        var novaArea = new
        {
            nome = "Area de Teste Automatizado",
            bioma = "Pantanal",
            municipio = "Corumba",
            uf = "MS",
            latitudeCentro = -19.0m,
            longitudeCentro = -57.6m,
            areaHectares = 1000m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/areas", novaArea);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var corpo = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(corpo);
        var id = doc.RootElement.GetProperty("id").GetInt32();
        Assert.True(id > 0, "A area criada deve ter um Id positivo.");
        Assert.Equal("Area de Teste Automatizado", doc.RootElement.GetProperty("nome").GetString());
    }
}
