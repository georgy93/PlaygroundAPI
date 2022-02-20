namespace Playground.Domain.Exceptions
{
    using System;

    public class CardExpiredException : BusinessException
    {
        public CardExpiredException(DateTime expirationDate) : base($"Card has expired at {expirationDate}")
        {
        }
    }
}