using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Data;

/// <summary>
/// Popula a base com a mesma micro-historia do script da Parte 1: o sensor PNT-S001
/// registra um evento de fogo, o satelite VIIRS confirma o mesmo ponto e isso gera um
/// alerta CRITICO de origem FUSAO, com a brigada de Corumba despachada. Assim os
/// endpoints retornam dados coerentes para o demo e para os prints da Parte 3.
/// </summary>
public static class DbSeeder
{
    public static void Seed(SentinelaDbContext db)
    {
        if (db.Areas.Any())
        {
            return; // ja semeado
        }

        var agora = DateTime.UtcNow;

        // ---- Areas monitoradas ----
        var corumba = new AreaMonitorada
        {
            Nome = "Corumba - Setor Rio Paraguai",
            Bioma = "Pantanal",
            Municipio = "Corumba",
            Uf = "MS",
            LatitudeCentro = -19.009200m,
            LongitudeCentro = -57.651300m,
            AreaHectares = 185000m,
            BboxNorte = -18.80m,
            BboxSul = -19.25m,
            BboxLeste = -57.40m,
            BboxOeste = -57.95m
        };
        var pocone = new AreaMonitorada
        {
            Nome = "Parque Nacional do Pantanal Matogrossense",
            Bioma = "Pantanal",
            Municipio = "Pocone",
            Uf = "MT",
            LatitudeCentro = -16.250000m,
            LongitudeCentro = -56.620000m,
            AreaHectares = 135000m,
            BboxNorte = -16.05m,
            BboxSul = -16.50m,
            BboxLeste = -56.40m,
            BboxOeste = -56.85m
        };
        var aquidauana = new AreaMonitorada
        {
            Nome = "Aquidauana - Borda Sul",
            Bioma = "Pantanal",
            Municipio = "Aquidauana",
            Uf = "MS",
            LatitudeCentro = -20.470000m,
            LongitudeCentro = -55.787000m,
            AreaHectares = 92000m,
            BboxNorte = -20.30m,
            BboxSul = -20.65m,
            BboxLeste = -55.55m,
            BboxOeste = -56.00m
        };

        // ---- Sensores ----
        var s001 = new Sensor
        {
            Codigo = "PNT-S001",
            Area = corumba,
            Latitude = -19.012000m,
            Longitude = -57.640000m,
            Conectividade = Conectividade.Lora,
            FonteEnergia = FonteEnergia.Solar,
            Status = StatusSensor.Ativo,
            DataInstalacao = agora.AddMonths(-8)
        };
        var s002 = new Sensor
        {
            Codigo = "PNT-S002",
            Area = corumba,
            Latitude = -19.030000m,
            Longitude = -57.680000m,
            Conectividade = Conectividade.Lora,
            FonteEnergia = FonteEnergia.Solar,
            Status = StatusSensor.Ativo,
            DataInstalacao = agora.AddMonths(-8)
        };
        var s003 = new Sensor
        {
            Codigo = "PNT-S003",
            Area = corumba,
            Latitude = -18.990000m,
            Longitude = -57.620000m,
            Conectividade = Conectividade.Celular4G,
            FonteEnergia = FonteEnergia.Bateria,
            Status = StatusSensor.Manutencao,
            DataInstalacao = agora.AddMonths(-5)
        };
        var s010 = new Sensor
        {
            Codigo = "PNT-S010",
            Area = pocone,
            Latitude = -16.255000m,
            Longitude = -56.610000m,
            Conectividade = Conectividade.Satelite,
            FonteEnergia = FonteEnergia.Solar,
            Status = StatusSensor.Ativo,
            DataInstalacao = agora.AddMonths(-3)
        };

        // ---- Leituras ----
        // Leituras normais do sensor 1 (3h atras)
        var leituras = new List<Leitura>
        {
            new() { Sensor = s001, TipoMedida = TipoMedida.Temperatura, Valor = 31.5m, Unidade = "C",   DataHora = agora.AddHours(-3) },
            new() { Sensor = s001, TipoMedida = TipoMedida.Umidade,     Valor = 38.0m, Unidade = "%",   DataHora = agora.AddHours(-3) },
            new() { Sensor = s001, TipoMedida = TipoMedida.Fumaca,      Valor = 12.0m, Unidade = "ppm", DataHora = agora.AddHours(-3) },
            // Evento de fogo no sensor 1 (20 min atras): temp alta + umidade baixa + fumaca alta
            new() { Sensor = s001, TipoMedida = TipoMedida.Temperatura, Valor = 58.7m,  Unidade = "C",   DataHora = agora.AddMinutes(-20) },
            new() { Sensor = s001, TipoMedida = TipoMedida.Umidade,     Valor = 14.0m,  Unidade = "%",   DataHora = agora.AddMinutes(-20) },
            new() { Sensor = s001, TipoMedida = TipoMedida.Fumaca,      Valor = 410.0m, Unidade = "ppm", DataHora = agora.AddMinutes(-20) }
        };

        // ---- Focos de calor ----
        var focoSatelite = new FocoCalor
        {
            Area = corumba,
            Fonte = FonteFoco.Satelite,
            Satelite = "VIIRS NOAA-20",
            Latitude = -19.013000m,
            Longitude = -57.641000m,
            TemperaturaBrilho = 342.10m,
            Frp = 28.4m,
            Confianca = 84,
            DataHoraDeteccao = agora.AddMinutes(-15)
        };
        var focoSolo = new FocoCalor
        {
            Area = corumba,
            Fonte = FonteFoco.SensorSolo,
            Satelite = null,
            Latitude = -19.012000m,
            Longitude = -57.640000m,
            Confianca = 90,
            DataHoraDeteccao = agora.AddMinutes(-18)
        };
        var focoModis = new FocoCalor
        {
            Area = pocone,
            Fonte = FonteFoco.Satelite,
            Satelite = "MODIS Aqua",
            Latitude = -16.252000m,
            Longitude = -56.615000m,
            TemperaturaBrilho = 318.40m,
            Frp = 9.1m,
            Confianca = 47,
            DataHoraDeteccao = agora.AddHours(-2)
        };

        // ---- Brigadas ----
        var brigadaCorumba = new Brigada
        {
            Nome = "Brigada Municipal Corumba 01",
            Tipo = TipoBrigada.Municipal,
            BaseLatitude = -19.009000m,
            BaseLongitude = -57.651000m,
            Contato = "(67) 99999-0001",
            Efetivo = 12,
            Status = StatusBrigada.EmOperacao
        };
        var brigadaIcmbio = new Brigada
        {
            Nome = "ICMBio - Base Pantanal",
            Tipo = TipoBrigada.Icmbio,
            BaseLatitude = -16.250000m,
            BaseLongitude = -56.620000m,
            Contato = "(65) 99999-0002",
            Efetivo = 8,
            Status = StatusBrigada.Disponivel
        };
        var brigadaVoluntaria = new Brigada
        {
            Nome = "Brigada Voluntaria Aquidauana",
            Tipo = TipoBrigada.Voluntaria,
            BaseLatitude = -20.470000m,
            BaseLongitude = -55.787000m,
            Contato = "(67) 99999-0003",
            Efetivo = 6,
            Status = StatusBrigada.Indisponivel
        };

        // ---- Alertas ----
        var alertaCritico = new Alerta
        {
            Area = corumba,
            Foco = focoSatelite,
            Nivel = NivelAlerta.Critico,
            Origem = OrigemAlerta.Fusao,
            Status = StatusAlerta.EmAtendimento,
            Descricao = "Foco confirmado por satelite VIIRS e por sensor PNT-S001 (temp 58.7C / umidade 14% / fumaca 410ppm).",
            DataHoraAbertura = agora.AddMinutes(-14)
        };
        var alertaMedio = new Alerta
        {
            Area = pocone,
            Foco = focoModis,
            Nivel = NivelAlerta.Medio,
            Origem = OrigemAlerta.Satelite,
            Status = StatusAlerta.Aberto,
            Descricao = "Hotspot MODIS com confianca 47%. Sem confirmacao de solo. Verificar.",
            DataHoraAbertura = agora.AddHours(-2)
        };

        // ---- Atendimento ----
        var atendimento = new Atendimento
        {
            Alerta = alertaCritico,
            Brigada = brigadaCorumba,
            Status = StatusAtendimento.EmCombate,
            DataHoraDespacho = agora.AddMinutes(-10),
            DataHoraChegada = agora.AddMinutes(-2)
        };

        db.AddRange(
            corumba, pocone, aquidauana,
            s001, s002, s003, s010,
            focoSatelite, focoSolo, focoModis,
            brigadaCorumba, brigadaIcmbio, brigadaVoluntaria,
            alertaCritico, alertaMedio,
            atendimento);
        db.AddRange(leituras);

        db.SaveChanges();
    }
}
