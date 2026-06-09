using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Common;

namespace Sentinela.Api.Middleware;

/// <summary>
/// Middleware global de tratamento de excecoes. Converte excecoes de dominio em
/// respostas HTTP padronizadas (RFC 7807 / ProblemDetails), evitando vazar stack
/// trace e mantendo o contrato de erro consistente em toda a API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await EscreverProblema(context, StatusCodes.Status404NotFound, "Recurso nao encontrado", ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await EscreverProblema(context, StatusCodes.Status409Conflict, "Regra de negocio violada", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro nao tratado ao processar {Path}", context.Request.Path);
            await EscreverProblema(context, StatusCodes.Status500InternalServerError,
                "Erro interno", "Ocorreu um erro inesperado ao processar a requisicao.");
        }
    }

    private static async Task EscreverProblema(HttpContext context, int status, string titulo, string detalhe)
    {
        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problema));
    }
}
