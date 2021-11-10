namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using SeedWork;
    using System;

    public class Buyer : Entity<int>, IAggregateRoot
    {
        // ef wants default constructor or one that accpets values for all parameters. The access modifier is not important
        protected Buyer() { }

        //  TODO: are names and email necessary?
        public Buyer(UserId userId, Email email, Name firstName, Name lastName)
        {
            //Id = userId?.ToString() ?? throw new ArgumentNullException(nameof(userId));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName;
            LastName = lastName;

            //EnsureValidState();
        }

        public Name FirstName { get; init; }

        public Name LastName { get; init; }

        public Email Email { get; init; }

        // public void ChangeEmail(Email newEmail) => ApplyChange(c => c.Email = newEmail);

        private void ApplyChange(Action<Buyer> changeAction)
        {
            if (changeAction == null)
                throw new ArgumentNullException(nameof(changeAction));

            changeAction(this);
            // EnsureValidState();
        }

        //private void EnsureValidState()
        //{
        //    if (string.IsNullOrWhiteSpace(Email))
        //        throw new InvalidEntityStateException(this, "Invalid state because of missing email");
        //}
    }
}
