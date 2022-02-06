namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using Ardalis.GuardClauses;
    using Exceptions;
    using SeedWork;
    using System;

    public class Buyer : Entity<int>, IAggregateRoot
    {
        // ef wants default constructor or one that accpets values for all parameters. The access modifier is not important
        protected Buyer() { }

        //  TODO: are names and email necessary?
        public Buyer(UserId userId, Email email, Name firstName, Name lastName)
        {
            Guard.Against.Null(userId, nameof(userId));
            Guard.Against.Null(email, nameof(email));
            Guard.Against.Null(firstName, nameof(firstName));
            Guard.Against.Null(lastName, nameof(lastName));

            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName;
            LastName = lastName;
        }

        public Name FirstName { get; private set; }

        public Name LastName { get; private set; }

        public Email Email { get; private set; }

        public void ChangeEmail(Email newEmail) => ApplyChange(c => c.Email = newEmail);

        private void ApplyChange(Action<Buyer> changeAction)
        {
            if (changeAction == null)
                throw new ArgumentNullException(nameof(changeAction));

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
