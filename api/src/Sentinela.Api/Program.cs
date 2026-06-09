using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sentinela.Api.Data;
using Sentinela.Api.Middleware;
using Sentinela.Api.Repositories;
using Sentinela.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON: enums como string e omite propriedades nulas
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SENTINELA API",
        Version = "v1",
        Description = "Deteccao precoce de incendios no Pantanal pela fusao de dados de " +
                      "satelite (NASA FIRMS) com sensores IoT em solo. FIAP Global Solution 2026/1."
    });

    // Comentarios XML aparecem na documentacao do Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Banco de dados. Provider escolhido pela chave "DatabaseProvider" do appsettings.
// InMemory (padrao) roda sem instalar nada; Oracle conecta no schema da Parte 1.
var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "InMemory";
builder.Services.AddDbContext<SentinelaDbContext>(options =>
{
    if (provider.Equals("Oracle", StringComparison.OrdinalIgnoreCase))
    {
        // Para usar Oracle: adicione o pacote Oracle.EntityFrameworkCore ao .csproj,
        // descomente a linha abaixo e configure ConnectionStrings:Oracle no appsettings.
        // options.UseOracle(builder.Configuration.GetConnectionString("Oracle"));
        throw new InvalidOperationException(
            "Provider 'Oracle' requer o pacote Oracle.EntityFrameworkCore. Veja o README (secao Banco de Dados).");
    }

    options.UseInMemoryDatabase("sentinela");
});

// Injecao de dependencia: repositorio generico + servicos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ILeituraService, LeituraService>();
builder.Services.AddScoped<IFocoService, FocoService>();
builder.Services.AddScoped<IBrigadaService, BrigadaService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IAtendimentoService, AtendimentoService>();
builder.Services.AddScoped<IDeteccaoService, DeteccaoService>();

var app = builder.Build();

// Cria e popula o banco com os dados de exemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SENTINELA API v1");
    options.DocumentTitle = "SENTINELA API";
});

// Raiz abre direto o Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();

// Necessario para os testes de integracao (WebApplicationFactory<Program>)
public partial class Program { }
