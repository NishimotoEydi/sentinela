namespace Sentinela.Api.Common;

/// <summary>Recurso nao encontrado. Mapeado para HTTP 404 pelo middleware.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public static NotFoundException For(string recurso, int id) =>
        new($"{recurso} com id {id} nao foi encontrado(a).");
}

/// <summary>Violacao de regra de negocio. Mapeado para HTTP 409 pelo middleware.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
