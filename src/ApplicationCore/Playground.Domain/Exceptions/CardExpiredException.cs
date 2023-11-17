namespace Playground.Domain.Exceptions;

public class CardExpiredException(DateTime expirationDate) : BusinessException($"Card has expired at {expirationDate}")
{ }