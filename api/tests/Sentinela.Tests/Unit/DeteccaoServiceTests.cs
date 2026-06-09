using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Data;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Repositories;
using Sentinela.Api.Services;
using Xunit;

namespace Sentinela.Tests.Unit;

/// <summary>
/// Testes unitarios da regra de deteccao em <see cref="DeteccaoService"/>.
/// Cada teste roda contra um banco InMemory isolado (nome unico via Guid), montando os
/// <see cref="Repository{T}"/> manualmente e semeando apenas os dados minimos do cenario.
/// </summary>
public class DeteccaoServiceTests
{
    /// <summary>Cria um DbContext InMemory totalmente isolado para o teste.</summary>
    private static SentinelaDbContext NovoContexto()
    {
        var options = new DbContextOptionsBuilder<SentinelaDbContext>()
            .UseInMemoryDatabase($"deteccao-tests-{Guid.NewGuid()}")
            .Options;
        return new SentinelaDbContext(options);
    }

    /// <summary>Monta o service com repositorios reais sobre o contexto informado.</summary>
    private static DeteccaoService NovoService(SentinelaDbContext db) => new(
        new Repository<Sensor>(db),
        new Repository<Leitura>(db),
        new Repository<FocoCalor>(db),
        new Repository<Alerta>(db));

    /// <summary>Adiciona as 3 leituras (temp/umidade/fumaca) recentes de um sensor.</summary>
    private static void SemearLeituras(
        SentinelaDbContext db, int sensorId, decimal temperatura, decimal umidade, decimal fumaca)
    {
        var agora = DateTime.UtcNow;
        db.Leituras.AddRange(
            new Leitura { SensorId = sensorId, TipoMedida = TipoMedida.Temperatura, Valor = temperatura, Unidade = "C", DataHora = agora.AddMinutes(-5) },
            new Leitura { SensorId = sensorId, TipoMedida = TipoMedida.Umidade, Valor = umidade, Unidade = "%", DataHora = agora.AddMinutes(-5) },
            new Leitura { SensorId = sensorId, TipoMedida = TipoMedida.Fumaca, Valor = fumaca, Unidade = "ppm", DataHora = agora.AddMinutes(-5) });
    }

    [Fact]
    public async Task AvaliarSensorAsync_FogoEmSoloComSateliteConfiavel_DeveGerarAlertaCriticoFusao()
    {
        // Arrange
        await using var db = NovoContexto();
        var area = new AreaMonitorada { Nome = "Area Teste", Municipio = "Corumba", Uf = "MS" };
        var sensor = new Sensor { Codigo = "TST-001", Area = area, Latitude = -19m, Longitude = -57m };
        db.AddRange(area, sensor);
        // Leituras que disparam a regra: temp >= 50, umidade <= 25, fumaca >= 100.
        SemearLeituras(db, sensorId: 0, temperatura: 60m, umidade: 12m, fumaca: 300m);
        // Foco de satelite na MESMA area, recente (15 min) e com confianca >= 50.
        db.FocosCalor.Add(new FocoCalor
        {
            Area = area,
            Fonte = FonteFoco.Satelite,
            Confianca = 84,
            DataHoraDeteccao = DateTime.UtcNow.AddMinutes(-15)
        });
        await db.SaveChangesAsync();
        // Corrige o SensorId das leituras agora que o sensor tem Id atribuido.
        foreach (var l in db.Leituras) l.SensorId = sensor.Id;
        await db.SaveChangesAsync();

        var service = NovoService(db);

        // Act
        var resultado = await service.AvaliarSensorAsync(sensor.Id);

        // Assert
        Assert.True(resultado.FogoDetectadoSolo);
        Assert.True(resultado.ConfirmadoPorSatelite);
        Assert.Equal(NivelAlerta.Critico, resultado.Nivel);
        Assert.Equal(OrigemAlerta.Fusao, resultado.Origem);
        Assert.NotNull(resultado.AlertaId);
        Assert.NotNull(resultado.FocoSoloId);
        // Persistiu o alerta critico de fusao no banco.
        Assert.Equal(1, await db.Alertas.CountAsync(a => a.Nivel == NivelAlerta.Critico && a.Origem == OrigemAlerta.Fusao));
    }

    [Fact]
    public async Task AvaliarSensorAsync_FogoEmSoloSemSatelite_DeveGerarAlertaAltoSensor()
    {
        // Arrange
        await using var db = NovoContexto();
        var area = new AreaMonitorada { Nome = "Area Teste", Municipio = "Pocone", Uf = "MT" };
        var sensor = new Sensor { Codigo = "TST-002", Area = area, Latitude = -16m, Longitude = -56m };
        db.AddRange(area, sensor);
        SemearLeituras(db, sensorId: 0, temperatura: 58m, umidade: 14m, fumaca: 410m);
        // Foco de satelite presente, porem com confianca ABAIXO do limiar (< 50) -> nao confirma.
        db.FocosCalor.Add(new FocoCalor
        {
            Area = area,
            Fonte = FonteFoco.Satelite,
            Confianca = 30,
            DataHoraDeteccao = DateTime.UtcNow.AddMinutes(-10)
        });
        await db.SaveChangesAsync();
        foreach (var l in db.Leituras) l.SensorId = sensor.Id;
        await db.SaveChangesAsync();

        var service = NovoService(db);

        // Act
        var resultado = await service.AvaliarSensorAsync(sensor.Id);

        // Assert
        Assert.True(resultado.FogoDetectadoSolo);
        Assert.False(resultado.ConfirmadoPorSatelite);
        Assert.Equal(NivelAlerta.Alto, resultado.Nivel);
        Assert.Equal(OrigemAlerta.Sensor, resultado.Origem);
        Assert.NotNull(resultado.AlertaId);
    }

    [Fact]
    public async Task AvaliarSensorAsync_LeiturasAbaixoDoLimiar_NaoDeveGerarAlerta()
    {
        // Arrange
        await using var db = NovoContexto();
        var area = new AreaMonitorada { Nome = "Area Teste", Municipio = "Aquidauana", Uf = "MS" };
        var sensor = new Sensor { Codigo = "TST-003", Area = area, Latitude = -20m, Longitude = -55m };
        db.AddRange(area, sensor);
        // Condicoes normais: longe dos limiares de fogo.
        SemearLeituras(db, sensorId: 0, temperatura: 31m, umidade: 60m, fumaca: 10m);
        await db.SaveChangesAsync();
        foreach (var l in db.Leituras) l.SensorId = sensor.Id;
        await db.SaveChangesAsync();

        var service = NovoService(db);

        // Act
        var resultado = await service.AvaliarSensorAsync(sensor.Id);

        // Assert
        Assert.False(resultado.FogoDetectadoSolo);
        Assert.Null(resultado.AlertaId);
        Assert.Null(resultado.Nivel);
        Assert.Equal(0, await db.Alertas.CountAsync());
    }

    [Fact]
    public async Task AvaliarSensorAsync_SensorInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        await using var db = NovoContexto();
        var service = NovoService(db);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.AvaliarSensorAsync(9999));
    }
}
