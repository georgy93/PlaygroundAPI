namespace Playground.Domain.Exceptions;

using Microsoft.AspNetCore.Http;

public abstract class BusinessException(string message) : ApplicationException(message)
{
    public virtual int HttpStatusCode => StatusCodes.Status500InternalServerError;
}