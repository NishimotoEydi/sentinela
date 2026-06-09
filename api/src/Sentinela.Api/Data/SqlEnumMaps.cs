using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sentinela.Api.Domain.Enums;

namespace Sentinela.Api.Data;

/// <summary>
/// Converte os enums para exatamente os mesmos textos usados nos CHECK do script
/// .sql da Parte 1 (ex.: EM_ATENDIMENTO, SENSOR_SOLO, 4G, FUSAO). Assim o banco
/// fica fiel ao modelo entregue no SQL, que e a fonte de verdade do projeto.
/// </summary>
internal static class SqlEnumMaps
{
    public static readonly ValueConverter<Conectividade, string> ConectividadeConv = Map(
        (Conectividade.Lora, "LORA"),
        (Conectividade.Celular4G, "4G"),
        (Conectividade.Satelite, "SATELITE"));

    public static readonly ValueConverter<FonteEnergia, string> FonteEnergiaConv = Map(
        (FonteEnergia.Solar, "SOLAR"),
        (FonteEnergia.Bateria, "BATERIA"),
        (FonteEnergia.Rede, "REDE"));

    public static readonly ValueConverter<StatusSensor, string> StatusSensorConv = Map(
        (StatusSensor.Ativo, "ATIVO"),
        (StatusSensor.Inativo, "INATIVO"),
        (StatusSensor.Manutencao, "MANUTENCAO"),
        (StatusSensor.Offline, "OFFLINE"));

    public static readonly ValueConverter<TipoMedida, string> TipoMedidaConv = Map(
        (TipoMedida.Temperatura, "TEMPERATURA"),
        (TipoMedida.Umidade, "UMIDADE"),
        (TipoMedida.Fumaca, "FUMACA"));

    public static readonly ValueConverter<FonteFoco, string> FonteFocoConv = Map(
        (FonteFoco.Satelite, "SATELITE"),
        (FonteFoco.SensorSolo, "SENSOR_SOLO"));

    public static readonly ValueConverter<NivelAlerta, string> NivelAlertaConv = Map(
        (NivelAlerta.Baixo, "BAIXO"),
        (NivelAlerta.Medio, "MEDIO"),
        (NivelAlerta.Alto, "ALTO"),
        (NivelAlerta.Critico, "CRITICO"));

    public static readonly ValueConverter<OrigemAlerta, string> OrigemAlertaConv = Map(
        (OrigemAlerta.Satelite, "SATELITE"),
        (OrigemAlerta.Sensor, "SENSOR"),
        (OrigemAlerta.Fusao, "FUSAO"));

    public static readonly ValueConverter<StatusAlerta, string> StatusAlertaConv = Map(
        (StatusAlerta.Aberto, "ABERTO"),
        (StatusAlerta.EmAtendimento, "EM_ATENDIMENTO"),
        (StatusAlerta.Resolvido, "RESOLVIDO"),
        (StatusAlerta.FalsoPositivo, "FALSO_POSITIVO"));

    public static readonly ValueConverter<TipoBrigada, string> TipoBrigadaConv = Map(
        (TipoBrigada.Municipal, "MUNICIPAL"),
        (TipoBrigada.Estadual, "ESTADUAL"),
        (TipoBrigada.Icmbio, "ICMBIO"),
        (TipoBrigada.Ibama, "IBAMA"),
        (TipoBrigada.Voluntaria, "VOLUNTARIA"));

    public static readonly ValueConverter<StatusBrigada, string> StatusBrigadaConv = Map(
        (StatusBrigada.Disponivel, "DISPONIVEL"),
        (StatusBrigada.EmOperacao, "EM_OPERACAO"),
        (StatusBrigada.Indisponivel, "INDISPONIVEL"));

    public static readonly ValueConverter<StatusAtendimento, string> StatusAtendimentoConv = Map(
        (StatusAtendimento.Despachado, "DESPACHADO"),
        (StatusAtendimento.EmDeslocamento, "EM_DESLOCAMENTO"),
        (StatusAtendimento.EmCombate, "EM_COMBATE"),
        (StatusAtendimento.Concluido, "CONCLUIDO"),
        (StatusAtendimento.Cancelado, "CANCELADO"));

    private static ValueConverter<T, string> Map<T>(params (T value, string token)[] pares)
        where T : struct, Enum
    {
        var paraToken = pares.ToDictionary(p => p.value, p => p.token);
        var paraEnum = pares.ToDictionary(p => p.token, p => p.value);
        return new ValueConverter<T, string>(v => paraToken[v], s => paraEnum[s]);
    }
}
