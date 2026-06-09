using Microsoft.EntityFrameworkCore;
using Sentinela.Api.Domain.Entities;

namespace Sentinela.Api.Data;

/// <summary>
/// Contexto EF Core do SENTINELA. Mapeia as 7 entidades para as mesmas tabelas e
/// colunas do script .sql da Parte 1 (AREA_MONITORADA, SENSOR, LEITURA, FOCO_CALOR,
/// BRIGADA, ALERTA, ATENDIMENTO). Os enums sao gravados com os mesmos textos dos
/// CHECK do SQL (ver SqlEnumMaps), mantendo o banco fiel ao modelo entregue.
/// </summary>
public class SentinelaDbContext : DbContext
{
    public SentinelaDbContext(DbContextOptions<SentinelaDbContext> options) : base(options)
    {
    }

    public DbSet<AreaMonitorada> Areas => Set<AreaMonitorada>();
    public DbSet<Sensor> Sensores => Set<Sensor>();
    public DbSet<Leitura> Leituras => Set<Leitura>();
    public DbSet<FocoCalor> FocosCalor => Set<FocoCalor>();
    public DbSet<Brigada> Brigadas => Set<Brigada>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AreaMonitorada>(e =>
        {
            e.ToTable("AREA_MONITORADA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_area");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
            e.Property(x => x.Bioma).HasColumnName("bioma").HasMaxLength(40).IsRequired();
            e.Property(x => x.Municipio).HasColumnName("municipio").HasMaxLength(80).IsRequired();
            e.Property(x => x.Uf).HasColumnName("uf").HasMaxLength(2).IsRequired();
            e.Property(x => x.LatitudeCentro).HasColumnName("latitude_centro").HasPrecision(9, 6);
            e.Property(x => x.LongitudeCentro).HasColumnName("longitude_centro").HasPrecision(9, 6);
            e.Property(x => x.AreaHectares).HasColumnName("area_hectares").HasPrecision(12, 2);
            e.Property(x => x.BboxNorte).HasColumnName("bbox_norte").HasPrecision(9, 6);
            e.Property(x => x.BboxSul).HasColumnName("bbox_sul").HasPrecision(9, 6);
            e.Property(x => x.BboxLeste).HasColumnName("bbox_leste").HasPrecision(9, 6);
            e.Property(x => x.BboxOeste).HasColumnName("bbox_oeste").HasPrecision(9, 6);
            e.Property(x => x.DataCadastro).HasColumnName("data_cadastro");
        });

        modelBuilder.Entity<Sensor>(e =>
        {
            e.ToTable("SENSOR");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_sensor");
            e.Property(x => x.AreaId).HasColumnName("id_area");
            e.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(30).IsRequired();
            e.HasIndex(x => x.Codigo).IsUnique();
            e.Property(x => x.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
            e.Property(x => x.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
            e.Property(x => x.Conectividade).HasColumnName("conectividade")
                .HasConversion(SqlEnumMaps.ConectividadeConv).HasMaxLength(15);
            e.Property(x => x.FonteEnergia).HasColumnName("fonte_energia")
                .HasConversion(SqlEnumMaps.FonteEnergiaConv).HasMaxLength(15);
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(SqlEnumMaps.StatusSensorConv).HasMaxLength(15);
            e.Property(x => x.DataInstalacao).HasColumnName("data_instalacao");
            e.HasOne(x => x.Area)
                .WithMany(a => a.Sensores)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Leitura>(e =>
        {
            e.ToTable("LEITURA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_leitura");
            e.Property(x => x.SensorId).HasColumnName("id_sensor");
            e.Property(x => x.TipoMedida).HasColumnName("tipo_medida")
                .HasConversion(SqlEnumMaps.TipoMedidaConv).HasMaxLength(15);
            e.Property(x => x.Valor).HasColumnName("valor").HasPrecision(8, 2);
            e.Property(x => x.Unidade).HasColumnName("unidade").HasMaxLength(10).IsRequired();
            e.Property(x => x.DataHora).HasColumnName("data_hora");
            e.HasOne(x => x.Sensor)
                .WithMany(s => s.Leituras)
                .HasForeignKey(x => x.SensorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FocoCalor>(e =>
        {
            e.ToTable("FOCO_CALOR");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_foco");
            e.Property(x => x.AreaId).HasColumnName("id_area");
            e.Property(x => x.Fonte).HasColumnName("fonte")
                .HasConversion(SqlEnumMaps.FonteFocoConv).HasMaxLength(15);
            e.Property(x => x.Satelite).HasColumnName("satelite").HasMaxLength(20);
            e.Property(x => x.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
            e.Property(x => x.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
            e.Property(x => x.TemperaturaBrilho).HasColumnName("temperatura_brilho").HasPrecision(7, 2);
            e.Property(x => x.Frp).HasColumnName("frp").HasPrecision(8, 2);
            e.Property(x => x.Confianca).HasColumnName("confianca");
            e.Property(x => x.DataHoraDeteccao).HasColumnName("data_hora_deteccao");
            e.HasOne(x => x.Area)
                .WithMany(a => a.FocosCalor)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Brigada>(e =>
        {
            e.ToTable("BRIGADA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_brigada");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(80).IsRequired();
            e.Property(x => x.Tipo).HasColumnName("tipo")
                .HasConversion(SqlEnumMaps.TipoBrigadaConv).HasMaxLength(15);
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(SqlEnumMaps.StatusBrigadaConv).HasMaxLength(15);
            e.Property(x => x.BaseLatitude).HasColumnName("base_latitude").HasPrecision(9, 6);
            e.Property(x => x.BaseLongitude).HasColumnName("base_longitude").HasPrecision(9, 6);
            e.Property(x => x.Contato).HasColumnName("contato").HasMaxLength(20);
            e.Property(x => x.Efetivo).HasColumnName("efetivo");
        });

        modelBuilder.Entity<Alerta>(e =>
        {
            e.ToTable("ALERTA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_alerta");
            e.Property(x => x.AreaId).HasColumnName("id_area");
            e.Property(x => x.FocoId).HasColumnName("id_foco");
            e.Property(x => x.Nivel).HasColumnName("nivel")
                .HasConversion(SqlEnumMaps.NivelAlertaConv).HasMaxLength(10);
            e.Property(x => x.Origem).HasColumnName("origem")
                .HasConversion(SqlEnumMaps.OrigemAlertaConv).HasMaxLength(10);
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(SqlEnumMaps.StatusAlertaConv).HasMaxLength(15);
            e.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(300);
            e.Property(x => x.DataHoraAbertura).HasColumnName("data_hora_abertura");
            e.Property(x => x.DataHoraResolucao).HasColumnName("data_hora_resolucao");
            e.HasOne(x => x.Area)
                .WithMany(a => a.Alertas)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Foco)
                .WithMany(f => f.Alertas)
                .HasForeignKey(x => x.FocoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Atendimento>(e =>
        {
            e.ToTable("ATENDIMENTO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id_atendimento");
            e.Property(x => x.AlertaId).HasColumnName("id_alerta");
            e.Property(x => x.BrigadaId).HasColumnName("id_brigada");
            e.Property(x => x.Status).HasColumnName("status")
                .HasConversion(SqlEnumMaps.StatusAtendimentoConv).HasMaxLength(15);
            e.Property(x => x.DataHoraDespacho).HasColumnName("data_hora_despacho");
            e.Property(x => x.DataHoraChegada).HasColumnName("data_hora_chegada");
            e.Property(x => x.DataHoraConclusao).HasColumnName("data_hora_conclusao");
            e.Property(x => x.Observacoes).HasColumnName("observacoes").HasMaxLength(300);
            e.HasOne(x => x.Alerta)
                .WithMany(a => a.Atendimentos)
                .HasForeignKey(x => x.AlertaId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Brigada)
                .WithMany(b => b.Atendimentos)
                .HasForeignKey(x => x.BrigadaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
