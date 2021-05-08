namespace Playground.Domain.Exceptions
{
    using System;

    public abstract class BusinessException : ApplicationException
    {
        public abstract int HttpStatusCode { get; }
    }
}