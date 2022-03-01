namespace Playground.Domain.Exceptions
{
    public class CardExpiredException : BusinessException
    {
        public CardExpiredException(DateTime expirationDate) : base($"Card has expired at {expirationDate}")
        {
        }
    }
}