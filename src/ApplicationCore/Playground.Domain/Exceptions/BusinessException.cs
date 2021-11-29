namespace Playground.Domain.Exceptions
{
    using System;

    public abstract class BusinessException : ApplicationException
    {
        public BusinessException(string message) : base(message)
        { }

        public abstract int HttpStatusCode { get; }
    }
}