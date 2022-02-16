namespace Playground.Domain.SeedWork
{
    public interface IAggregateRoot : IDomainEntity
    {
        public void IncreaseVersion() { }
    }
}