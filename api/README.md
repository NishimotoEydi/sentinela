# SENTINELA - API REST (Parte 2)

API do projeto SENTINELA, da Global Solution 2026/1 (FIAP, Engenharia de Software). O sistema faz detecção precoce de incêndios no Pantanal cruzando focos de calor de satélite (NASA FIRMS) com leituras de sensores IoT em solo.

Esta pasta contém a Parte 2 (Implementação dos Serviços / API). É uma API RESTful em C# com ASP.NET Core 8, organizada nas camadas Controller, Service e Repository, usando Entity Framework Core para o acesso a dados e Swagger para a documentação.

## Como rodar

Precisa do SDK do .NET 8 (https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
# dentro da pasta /api
dotnet run --project src/Sentinela.Api
```

A API sobe em `http://localhost:5080` e a raiz já abre o Swagger:

- Swagger UI: http://localhost:5080/swagger
- OpenAPI JSON: http://localhost:5080/swagger/v1/swagger.json

Por padrão usamos um banco em memória (EF Core InMemory) já populado com os dados de exemplo do Pantanal, os mesmos do script da Parte 1. Assim dá pra rodar e testar sem instalar nenhum banco. Para conectar no Oracle, veja a seção "Banco de dados".

Para rodar os testes (Parte 3):

```bash
dotnet test
```

## Estrutura

A separação em camadas segue o que o edital pede (Controller / Service / Repository):

```
HTTP -> Controller -> Service -> Repository -> DbContext (EF Core)
```

- `src/Sentinela.Api/Controllers` - endpoints REST, recebem o request e chamam o service.
- `src/Sentinela.Api/Services` - regras de negócio, validações de domínio e a lógica de detecção.
- `src/Sentinela.Api/Repositories` - `IRepository<T>` genérico em cima do EF Core.
- `src/Sentinela.Api/Domain` - entidades e enums (espelham o modelo da Parte 1).
- `src/Sentinela.Api/Data` - DbContext, mapeamentos e o seed dos dados.
- `src/Sentinela.Api/Dtos` e `Mapping` - objetos de entrada/saída e a conversão para as entidades.
- `src/Sentinela.Api/Middleware` e `Common` - tratamento de erros em um ponto só (respostas em ProblemDetails).

Algumas escolhas: DTOs separados das entidades (a API não expõe o modelo do banco), injeção de dependência nos services e no repositório, enums gravados e serializados como texto, `async/await` na pilha de I/O e validação com DataAnnotations (o `[ApiController]` já devolve 400 quando o corpo é inválido).

## Endpoints

São 8 recursos e 31 operações no total, cobrindo GET, POST, PUT e DELETE (o edital pede pelo menos 5 endpoints).

| Recurso | Operações |
|---|---|
| Áreas monitoradas | `GET /api/areas`, `GET /api/areas/{id}`, `POST`, `PUT /api/areas/{id}`, `DELETE /api/areas/{id}` |
| Sensores | `GET /api/sensores` (filtros `areaId`, `status`), `GET /{id}`, `POST`, `PUT /{id}`, `PUT /{id}/status`, `DELETE /{id}` |
| Leituras (IoT) | `GET /api/leituras` (filtro `sensorId`), `GET /{id}`, `POST` |
| Focos de calor | `GET /api/focos` (filtros `areaId`, `desde`), `GET /{id}`, `POST` |
| Alertas | `GET /api/alertas` (filtros `nivel`, `status`, `areaId`), `GET /{id}`, `POST`, `PUT /{id}/status`, `DELETE /{id}` |
| Brigadas | `GET /api/brigadas` (filtro `status`), `GET /{id}`, `POST`, `PUT /{id}/status` |
| Atendimentos | `GET /api/atendimentos` (filtro `alertaId`), `GET /{id}`, `POST`, `PUT /{id}/status` |
| Detecção | `POST /api/deteccao/avaliar/{sensorId}` |

Dá pra testar tudo pelo Swagger ou importando `postman/Sentinela.postman_collection.json` no Postman.

## Detecção (cruzamento satélite + IoT)

O endpoint `POST /api/deteccao/avaliar/{sensorId}` olha as leituras recentes do sensor (últimos 30 minutos) e aplica a regra de fogo em solo: temperatura maior ou igual a 50 °C, umidade menor ou igual a 25% e fumaça maior ou igual a 100 ppm.

- Se a regra bate, cria um foco de solo e abre um alerta.
- Se já existe um foco de satélite recente e com confiança boa (>= 50) na mesma área, o alerta sai como CRÍTICO de origem FUSAO.
- Se só o solo acusou, o alerta sai como ALTO de origem SENSOR.

Exemplo do fluxo:

```bash
# sensor manda as leituras de um evento de fogo
POST /api/leituras { "sensorId": 2, "tipoMedida": "Temperatura", "valor": 61.2, "unidade": "C" }
POST /api/leituras { "sensorId": 2, "tipoMedida": "Umidade",     "valor": 12.0, "unidade": "%" }
POST /api/leituras { "sensorId": 2, "tipoMedida": "Fumaca",      "valor": 350,  "unidade": "ppm" }

# a aplicação avalia e abre o alerta
POST /api/deteccao/avaliar/2
```

## Dados de exemplo

Ao subir, o banco é populado com a situação descrita na Parte 1: 3 áreas, 4 sensores, leituras normais mais um evento de fogo no sensor PNT-S001, focos de satélite (VIIRS e MODIS), 3 brigadas e um alerta crítico de origem FUSAO com a brigada de Corumbá já despachada. Com isso os endpoints retornam dados de verdade na hora da demonstração.

## Banco de dados

A escolha do banco está na chave `DatabaseProvider` do `appsettings.json`:

- `InMemory` (padrão): roda em qualquer máquina, sem instalar nada.
- `Oracle`: conecta no schema da Parte 1. Para habilitar:
  1. `dotnet add src/Sentinela.Api package Oracle.EntityFrameworkCore`
  2. no `Program.cs`, troque a linha do `throw` por `options.UseOracle(builder.Configuration.GetConnectionString("Oracle"));`
  3. ajuste `ConnectionStrings:Oracle` no `appsettings.json` (usuário e senha da FIAP).

O mapeamento do EF (no `SentinelaDbContext`) usa os mesmos nomes de tabela e coluna do script `sentinela_banco_de_dados.sql` (AREA_MONITORADA, id_area, latitude_centro, etc.) e grava os enums com os mesmos textos dos CHECK do SQL (CRITICO, FUSAO, EM_ATENDIMENTO, SENSOR_SOLO, 4G, ...). Ou seja, o modelo da API espelha o `.sql` da Parte 1; apontando para o Oracle, ele cai exatamente nesse schema. O `.sql` é a referência do modelo, não o diagrama.

Como o edital permite entregar as partes sem integração, deixamos o InMemory como padrão pra garantir que a API rode com um comando só.

## Tecnologias

.NET 8 / C#, ASP.NET Core Web API, Entity Framework Core 8 (InMemory, com Oracle opcional), Swagger (Swashbuckle) e Postman para a documentação, xUnit nos testes. Tudo dentro do que foi visto até a fase 4.
