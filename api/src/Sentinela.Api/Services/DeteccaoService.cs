using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Common;
using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;
using Sentinela.Api.Dtos;
using Sentinela.Api.Repositories;

namespace Sentinela.Api.Services;

public interface IDeteccaoService
{
    /// <summary>
    /// Avalia as leituras recentes de um sensor e, havendo indicio de fogo, cria um
    /// foco de solo e abre um alerta. Com confirmacao de satelite na regiao, o
    /// alerta vira CRITICO de origem FUSAO.
    /// </summary>
    Task<DeteccaoResultado> AvaliarSensorAsync(int sensorId, CancellationToken ct = default);
}

public class DeteccaoService : IDeteccaoService
{
    // Limiares da regra de fogo em solo (fumaca + temperatura alta + umidade baixa).
    private const decimal LimiarTemperaturaC = 50m;
    private const decimal LimiarUmidadePercent = 25m;
    private const decimal LimiarFumacaPpm = 100m;

    // Janelas de tempo da analise.
    private static readonly TimeSpan JanelaLeituras = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan JanelaConfirmacaoSatelite = TimeSpan.FromHours(6);
    private const int ConfiancaMinimaSatelite = 50;

    private readonly IRepository<Sensor> _sensores;
    private readonly IRepository<Leitura> _leituras;
    private readonly IRepository<FocoCalor> _focos;
    private readonly IRepository<Alerta> _alertas;

    public DeteccaoService(
        IRepository<Sensor> sensores,
        IRepository<Leitura> leituras,
        IRepository<FocoCalor> focos,
        IRepository<Alerta> alertas)
    {
        _sensores = sensores;
        _leituras = leituras;
        _focos = focos;
        _alertas = alertas;
    }

    public async Task<DeteccaoResultado> AvaliarSensorAsync(int sensorId, CancellationToken ct = default)
    {
        var sensor = await _sensores.GetByIdAsync(sensorId, ct)
                     ?? throw NotFoundException.For("Sensor", sensorId);

        var limiteJanela = DateTime.UtcNow - JanelaLeituras;
        var recentes = await _leituras.Query().AsNoTracking()
            .Where(l => l.SensorId == sensorId && l.DataHora >= limiteJanela)
            .ToListAsync(ct);

        var temperatura = UltimoValor(recentes, TipoMedida.Temperatura);
        var umidade = UltimoValor(recentes, TipoMedida.Umidade);
        var fumaca = UltimoValor(recentes, TipoMedida.Fumaca);

        var fogoEmSolo =
            temperatura is >= LimiarTemperaturaC &&
            umidade is <= LimiarUmidadePercent &&
            fumaca is >= LimiarFumacaPpm;

        if (!fogoEmSolo)
        {
            return new DeteccaoResultado
            {
                SensorId = sensorId,
                FogoDetectadoSolo = false,
                Mensagem = "Sem indicio de fogo nas leituras recentes do sensor."
            };
        }

        // 1) Cria o foco derivado do solo.
        var focoSolo = new FocoCalor
        {
            AreaId = sensor.AreaId,
            Fonte = FonteFoco.SensorSolo,
            Latitude = sensor.Latitude,
            Longitude = sensor.Longitude,
            Confianca = 90,
            DataHoraDeteccao = DateTime.UtcNow
        };
        await _focos.AddAsync(focoSolo, ct);

        // 2) Procura confirmacao por satelite na mesma area, recente e com boa confianca.
        var limiteSatelite = DateTime.UtcNow - JanelaConfirmacaoSatelite;
        var confirmadoPorSatelite = await _focos.Query().AsNoTracking().AnyAsync(f =>
            f.AreaId == sensor.AreaId &&
            f.Fonte == FonteFoco.Satelite &&
            f.DataHoraDeteccao >= limiteSatelite &&
            (f.Confianca ?? 0) >= ConfiancaMinimaSatelite, ct);

        // 3) Decide nivel e origem: fusao (satelite + solo) => CRITICO.
        var nivel = confirmadoPorSatelite ? NivelAlerta.Critico : NivelAlerta.Alto;
        var origem = confirmadoPorSatelite ? OrigemAlerta.Fusao : OrigemAlerta.Sensor;

        var descricao =
            $"Sensor {sensor.Codigo}: temp {temperatura}C / umidade {umidade}% / fumaca {fumaca}ppm. " +
            (confirmadoPorSatelite
                ? "Confirmado por foco de satelite na area (FUSAO)."
                : "Sem confirmacao de satelite ate o momento.");

        var alerta = new Alerta
        {
            AreaId = sensor.AreaId,
            Foco = focoSolo,
            Nivel = nivel,
            Origem = origem,
            Status = StatusAlerta.Aberto,
            Descricao = descricao,
            DataHoraAbertura = DateTime.UtcNow
        };
        await _alertas.AddAsync(alerta, ct);

        await _alertas.SaveChangesAsync(ct);

        return new DeteccaoResultado
        {
            SensorId = sensorId,
            FogoDetectadoSolo = true,
            ConfirmadoPorSatelite = confirmadoPorSatelite,
            Mensagem = confirmadoPorSatelite
                ? "Fogo detectado em solo e confirmado por satelite: alerta CRITICO (FUSAO) gerado."
                : "Fogo detectado em solo: alerta ALTO gerado, aguardando confirmacao de satelite.",
            FocoSoloId = focoSolo.Id,
            AlertaId = alerta.Id,
            Nivel = nivel,
            Origem = origem
        };
    }

    /// <summary>Valor da leitura mais recente de um tipo dentro da janela analisada.</summary>
    private static decimal? UltimoValor(IEnumerable<Leitura> leituras, TipoMedida tipo) =>
        leituras
            .Where(l => l.TipoMedida == tipo)
            .OrderByDescending(l => l.DataHora)
            .Select(l => (decimal?)l.Valor)
            .FirstOrDefault();
}
