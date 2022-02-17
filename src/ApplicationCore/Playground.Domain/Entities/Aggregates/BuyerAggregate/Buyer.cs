namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using Ardalis.GuardClauses;
    using Exceptions;
    using SeedWork;
    using System;

    public class Buyer : AggregateRootBase<int>
    {
        // ef wants default constructor or one that accpets values for all parameters. The access modifier is not important
        protected Buyer() { }

        //  TODO: are names and email necessary?
        public Buyer(UserId userId, Email email, FullName fullName)
        {
            Guard.Against.Null(userId);

            Email = Guard.Against.Null(email);
            FullName = Guard.Against.Null(fullName);
        }

        public FullName FullName { get; private set; }

        public Email Email { get; private set; }

        public void ChangeEmail(Email newEmail) => ApplyChange(c => c.Email = newEmail);

        private void ApplyChange(Action<Buyer> changeAction)
        {
            Guard.Against.Null(changeAction);

            changeAction(this);
            EnsureValidState();
        }

        private void EnsureValidState()
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new InvalidEntityStateException(this, "Invalid state because of missing email");
        }
    }
}
