namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;

    public record OrderStartedDomainEvent(
        Order Order,
        string UserId,
        string UserName,
        int CardTypeId,
        string CardNumber,
        string CardSecurityNumber,
        string CardHolderName,
        DateTime CardExpiration) : INotification;
}