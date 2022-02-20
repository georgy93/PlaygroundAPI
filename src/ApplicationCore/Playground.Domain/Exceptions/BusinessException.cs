namespace Playground.Domain.Exceptions
{
    using Microsoft.AspNetCore.Http;
    using System;

    public abstract class BusinessException : ApplicationException
    {
        public BusinessException(string message) : base(message)
        { }

        public virtual int HttpStatusCode => StatusCodes.Status500InternalServerError;
    }
}