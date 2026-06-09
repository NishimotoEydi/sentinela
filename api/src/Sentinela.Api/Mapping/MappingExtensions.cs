using Sentinela.Api.Domain.Entities;
using Sentinela.Api.Dtos;

namespace Sentinela.Api.Mapping;

/// <summary>
/// Mapeamento manual entidade &lt;-&gt; DTO. Optou-se por mapeamento explicito (sem
/// bibliotecas externas) para manter a solucao dentro do conteudo ministrado e
/// tornar a transformacao transparente para quem le o codigo.
/// </summary>
public static class MappingExtensions
{
    // ---------------- Area ----------------
    public static AreaResponse ToResponse(this AreaMonitorada e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Bioma = e.Bioma,
        Municipio = e.Municipio,
        Uf = e.Uf,
        LatitudeCentro = e.LatitudeCentro,
        LongitudeCentro = e.LongitudeCentro,
        AreaHectares = e.AreaHectares,
        BboxNorte = e.BboxNorte,
        BboxSul = e.BboxSul,
        BboxLeste = e.BboxLeste,
        BboxOeste = e.BboxOeste,
        DataCadastro = e.DataCadastro
    };

    public static AreaMonitorada ToEntity(this AreaCreateRequest r) => new()
    {
        Nome = r.Nome,
        Bioma = string.IsNullOrWhiteSpace(r.Bioma) ? "Pantanal" : r.Bioma,
        Municipio = r.Municipio,
        Uf = r.Uf.ToUpperInvariant(),
        LatitudeCentro = r.LatitudeCentro,
        LongitudeCentro = r.LongitudeCentro,
        AreaHectares = r.AreaHectares,
        BboxNorte = r.BboxNorte,
        BboxSul = r.BboxSul,
        BboxLeste = r.BboxLeste,
        BboxOeste = r.BboxOeste
    };

    public static void ApplyTo(this AreaCreateRequest r, AreaMonitorada e)
    {
        e.Nome = r.Nome;
        e.Bioma = string.IsNullOrWhiteSpace(r.Bioma) ? "Pantanal" : r.Bioma;
        e.Municipio = r.Municipio;
        e.Uf = r.Uf.ToUpperInvariant();
        e.LatitudeCentro = r.LatitudeCentro;
        e.LongitudeCentro = r.LongitudeCentro;
        e.AreaHectares = r.AreaHectares;
        e.BboxNorte = r.BboxNorte;
        e.BboxSul = r.BboxSul;
        e.BboxLeste = r.BboxLeste;
        e.BboxOeste = r.BboxOeste;
    }

    // ---------------- Sensor ----------------
    public static SensorResponse ToResponse(this Sensor e) => new()
    {
        Id = e.Id,
        AreaId = e.AreaId,
        Codigo = e.Codigo,
        Latitude = e.Latitude,
        Longitude = e.Longitude,
        Conectividade = e.Conectividade,
        FonteEnergia = e.FonteEnergia,
        Status = e.Status,
        DataInstalacao = e.DataInstalacao
    };

    public static Sensor ToEntity(this SensorCreateRequest r) => new()
    {
        AreaId = r.AreaId,
        Codigo = r.Codigo,
        Latitude = r.Latitude,
        Longitude = r.Longitude,
        Conectividade = r.Conectividade,
        FonteEnergia = r.FonteEnergia,
        Status = r.Status
    };

    public static void ApplyTo(this SensorUpdateRequest r, Sensor e)
    {
        e.Codigo = r.Codigo;
        e.Latitude = r.Latitude;
        e.Longitude = r.Longitude;
        e.Conectividade = r.Conectividade;
        e.FonteEnergia = r.FonteEnergia;
        e.Status = r.Status;
    }

    // ---------------- Leitura ----------------
    public static LeituraResponse ToResponse(this Leitura e) => new()
    {
        Id = e.Id,
        SensorId = e.SensorId,
        TipoMedida = e.TipoMedida,
        Valor = e.Valor,
        Unidade = e.Unidade,
        DataHora = e.DataHora
    };

    public static Leitura ToEntity(this LeituraCreateRequest r) => new()
    {
        SensorId = r.SensorId,
        TipoMedida = r.TipoMedida!.Value,
        Valor = r.Valor,
        Unidade = r.Unidade,
        DataHora = r.DataHora ?? DateTime.UtcNow
    };

    // ---------------- Foco de calor ----------------
    public static FocoResponse ToResponse(this FocoCalor e) => new()
    {
        Id = e.Id,
        AreaId = e.AreaId,
        Fonte = e.Fonte,
        Satelite = e.Satelite,
        Latitude = e.Latitude,
        Longitude = e.Longitude,
        TemperaturaBrilho = e.TemperaturaBrilho,
        Frp = e.Frp,
        Confianca = e.Confianca,
        DataHoraDeteccao = e.DataHoraDeteccao
    };

    public static FocoCalor ToEntity(this FocoCreateRequest r) => new()
    {
        AreaId = r.AreaId,
        Fonte = r.Fonte!.Value,
        Satelite = r.Satelite,
        Latitude = r.Latitude,
        Longitude = r.Longitude,
        TemperaturaBrilho = r.TemperaturaBrilho,
        Frp = r.Frp,
        Confianca = r.Confianca,
        DataHoraDeteccao = r.DataHoraDeteccao ?? DateTime.UtcNow
    };

    // ---------------- Brigada ----------------
    public static BrigadaResponse ToResponse(this Brigada e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Tipo = e.Tipo,
        BaseLatitude = e.BaseLatitude,
        BaseLongitude = e.BaseLongitude,
        Contato = e.Contato,
        Efetivo = e.Efetivo,
        Status = e.Status
    };

    public static Brigada ToEntity(this BrigadaCreateRequest r) => new()
    {
        Nome = r.Nome,
        Tipo = r.Tipo!.Value,
        BaseLatitude = r.BaseLatitude,
        BaseLongitude = r.BaseLongitude,
        Contato = r.Contato,
        Efetivo = r.Efetivo,
        Status = r.Status
    };

    // ---------------- Alerta ----------------
    public static AlertaResponse ToResponse(this Alerta e) => new()
    {
        Id = e.Id,
        AreaId = e.AreaId,
        FocoId = e.FocoId,
        Nivel = e.Nivel,
        Origem = e.Origem,
        Status = e.Status,
        Descricao = e.Descricao,
        DataHoraAbertura = e.DataHoraAbertura,
        DataHoraResolucao = e.DataHoraResolucao
    };

    public static Alerta ToEntity(this AlertaCreateRequest r) => new()
    {
        AreaId = r.AreaId,
        FocoId = r.FocoId,
        Nivel = r.Nivel!.Value,
        Origem = r.Origem!.Value,
        Descricao = r.Descricao
    };

    // ---------------- Atendimento ----------------
    public static AtendimentoResponse ToResponse(this Atendimento e) => new()
    {
        Id = e.Id,
        AlertaId = e.AlertaId,
        BrigadaId = e.BrigadaId,
        Status = e.Status,
        DataHoraDespacho = e.DataHoraDespacho,
        DataHoraChegada = e.DataHoraChegada,
        DataHoraConclusao = e.DataHoraConclusao,
        Observacoes = e.Observacoes
    };

    public static Atendimento ToEntity(this AtendimentoCreateRequest r) => new()
    {
        AlertaId = r.AlertaId,
        BrigadaId = r.BrigadaId,
        Observacoes = r.Observacoes
    };
}
