namespace Sentinela.Api.Domain.Enums;

/// <summary>Situacao operacional de uma estacao sensora em campo.</summary>
public enum StatusSensor
{
    Ativo,
    Inativo,
    Manutencao,
    Offline
}

/// <summary>Meio de comunicacao usado pelo sensor para transmitir leituras.</summary>
public enum Conectividade
{
    Lora,
    Celular4G,
    Satelite
}

/// <summary>Fonte de energia da estacao sensora.</summary>
public enum FonteEnergia
{
    Solar,
    Bateria,
    Rede
}

/// <summary>Grandeza medida em uma leitura IoT.</summary>
public enum TipoMedida
{
    Temperatura,
    Umidade,
    Fumaca
}

/// <summary>Origem do foco de calor detectado.</summary>
public enum FonteFoco
{
    /// <summary>Hotspot observado por satelite (ex.: NASA FIRMS / VIIRS / MODIS).</summary>
    Satelite,

    /// <summary>Foco derivado da malha de sensores em solo.</summary>
    SensorSolo
}

/// <summary>Severidade do alerta.</summary>
public enum NivelAlerta
{
    Baixo,
    Medio,
    Alto,
    Critico
}

/// <summary>
/// Como o alerta foi gerado. <see cref="Fusao"/> indica que satelite e sensores
/// de solo concordam.
/// </summary>
public enum OrigemAlerta
{
    /// <summary>Apenas o satelite detectou (possivel falso-positivo).</summary>
    Satelite,

    /// <summary>Apenas o sensor de solo detectou.</summary>
    Sensor,

    /// <summary>Satelite + solo concordam (alta confianca).</summary>
    Fusao
}

/// <summary>Ciclo de vida de um alerta.</summary>
public enum StatusAlerta
{
    Aberto,
    EmAtendimento,
    Resolvido,
    FalsoPositivo
}

/// <summary>Esfera responsavel pela brigada de combate.</summary>
public enum TipoBrigada
{
    Municipal,
    Estadual,
    Icmbio,
    Ibama,
    Voluntaria
}

/// <summary>Disponibilidade de uma brigada.</summary>
public enum StatusBrigada
{
    Disponivel,
    EmOperacao,
    Indisponivel
}

/// <summary>Ciclo de vida do despacho de uma brigada para um alerta.</summary>
public enum StatusAtendimento
{
    Despachado,
    EmDeslocamento,
    EmCombate,
    Concluido,
    Cancelado
}
