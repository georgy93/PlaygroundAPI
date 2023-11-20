namespace Playground.Domain.SeedWork;

public interface IAggregateRoot : IDomainEntity
{
    void IncreaseVersion();
}