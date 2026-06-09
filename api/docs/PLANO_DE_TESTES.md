# SENTINELA - Plano de Testes (Parte 3)

Projeto: SENTINELA, detecção precoce de incêndios no Pantanal cruzando focos de calor de satélite (NASA FIRMS) com leituras de sensores IoT em solo.

Entregável: Parte 3, plano de testes automatizados.

Objetivo: validar de forma automatizada a regra de negócio principal (a detecção que cruza satélite e IoT) e o contrato HTTP da API (status codes e respostas), para ter confiança e pegar regressões.

Ferramentas: xUnit para os testes, `WebApplicationFactory<Program>` para subir a API nos testes de integração e EF Core InMemory como banco.

## Estratégia

Os testes ficam em duas pastas, acompanhando as camadas da API:

- Unitários (`tests/Sentinela.Tests/Unit/`): testam o `DeteccaoService` isolado. Cada teste cria um `SentinelaDbContext` InMemory com nome único (`Guid.NewGuid()`), monta os `Repository<T>` na mão e insere só os dados do cenário. Assim um teste não interfere no outro e o foco fica na regra.
- Integração (`tests/Sentinela.Tests/Integration/`): sobem a API de verdade com `WebApplicationFactory<Program>` e batem com um `HttpClient` no pipeline completo (validação, middleware de erro, serialização e o banco com seed). Os asserts olham status code e presença de dados, sem depender de contagem exata.

Os testes usam nomes no padrão `Metodo_Cenario_ResultadoEsperado`, organização Arrange/Act/Assert e só o `Assert` do xUnit.

## Casos de teste

| ID | Cenário | Pré-condição/Entrada | Saída esperada | Status (Passou/Falhou) |
|----|---------|----------------------|----------------|------------------------|
| CT-01 | Unit: fogo em solo com foco de satélite confiável na mesma área gera FUSAO | Sensor com leituras temp=60C, umidade=12%, fumaça=300ppm (acima dos limiares) e um foco de satélite na mesma área, há 15 min, confiança 84 | `FogoDetectadoSolo=true`, `ConfirmadoPorSatelite=true`, `Nivel=Critico`, `Origem=Fusao`; cria foco de solo e 1 alerta crítico | Passou |
| CT-02 | Unit: fogo em solo sem confirmação de satélite (confiança < 50) gera ALTO/SENSOR | Sensor com leituras temp=58C, umidade=14%, fumaça=410ppm e foco de satélite com confiança 30 | `FogoDetectadoSolo=true`, `ConfirmadoPorSatelite=false`, `Nivel=Alto`, `Origem=Sensor`; cria alerta | Passou |
| CT-03 | Unit: leituras abaixo do limiar não geram alerta | Sensor com leituras normais temp=31C, umidade=60%, fumaça=10ppm | `FogoDetectadoSolo=false`, `Nivel=null`, `AlertaId=null`; nenhum alerta no banco | Passou |
| CT-04 | Unit: sensor inexistente lança exceção | `AvaliarSensorAsync(9999)` em banco vazio | Lança `Sentinela.Api.Common.NotFoundException` | Passou |
| CT-05 | Integração: listar alertas filtrando por nível Crítico | `GET /api/alertas?nivel=Critico` (API com seed) | HTTP 200, array com pelo menos 1 alerta, contendo `"Critico"` e `"Fusao"` | Passou |
| CT-06 | Integração: criar leitura com corpo inválido | `POST /api/leituras` com `{"sensorId":1,"unidade":"C"}` (falta `TipoMedida` e `Valor`) | HTTP 400 (ProblemDetails) | Passou |
| CT-07 | Integração: buscar alerta inexistente | `GET /api/alertas/9999` | HTTP 404 | Passou |
| CT-08 | Integração: criar área válida | `POST /api/areas` com payload válido | HTTP 201, cabeçalho `Location` presente, corpo com `id > 0` | Passou |

## Como executar

Na pasta `api`:

```bash
# precisa do SDK do .NET 8 no PATH
export DOTNET_ROOT="$HOME/.dotnet" && export PATH="$HOME/.dotnet:$PATH"

# roda tudo
dotnet test

# roda gerando o relatório TRX e salvando a saída (evidências)
dotnet test --logger "trx;LogFileName=resultado-testes.trx" \
  --results-directory docs/evidencias 2>&1 | tee docs/evidencias/saida-dotnet-test.txt
```

O projeto de testes já está na solução `Sentinela.sln`, então o `dotnet test` na raiz acha e roda todos os casos.

## Evidências

As evidências da execução ficam em `docs/evidencias/`:

- `docs/evidencias/saida-dotnet-test.txt`: saída completa do `dotnet test`.
- `docs/evidencias/resultado-testes.trx`: relatório no formato TRX, com os contadores e o detalhe de cada teste.

Resumo da execução:

```
Aprovado!  – Com falha:     0, Aprovado:     8, Ignorado:     0, Total:     8, Duração: 80 ms - Sentinela.Tests.dll (net8.0)
```

Contadores do TRX:

```
<Counters total="8" executed="8" passed="8" failed="0" error="0" timeout="0" aborted="0" inconclusive="0" />
```

## Rastreabilidade (caso de teste x requisito do edital)

| Requisito (Parte 3) | Onde é atendido | Como |
|---------------------|-----------------|------|
| Pelo menos 5 casos de teste | CT-01 a CT-08 (8 casos) | 4 unitários da detecção mais 4 de integração HTTP |
| Pelo menos 3 casos executados | CT-01 a CT-08 (todos) | Todos rodaram e passaram (Failed: 0, Passed: 8) |
| Evidências da execução | `docs/evidencias/` | Saída do `dotnet test` e relatório TRX |
| Regra de negócio principal | CT-01 a CT-04 | Cobrem os caminhos da detecção: Fusao/Crítico, Sensor/Alto, sem fogo e sensor inexistente |
| Contrato da API (status codes) | CT-05 a CT-08 | Cobrem 200, 400, 404 e 201 |
