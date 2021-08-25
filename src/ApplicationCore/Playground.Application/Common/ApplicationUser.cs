namespace Playground.Application.Common
{
    using Domain.Entities.Aggregates.Buyer;
    using Domain.Exceptions;
    using Microsoft.AspNetCore.Identity;
    using System;

    // TODO: No value objects here
    // Imagine this is handled in another MS and we are reading information from token in this MS
    public class ApplicationUser : IdentityUser
    {
        // ef wants default constructor or one that accpets values for all parameters. The access modifier is not important
        protected ApplicationUser() { }

        public ApplicationUser(UserId userId, Email email)
        {
            Id = userId?.ToString() ?? throw new ArgumentNullException(nameof(userId));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UserName = email;

            EnsureValidState();
        }

        public void ChangeEmail(Email newEmail) => ApplyChange(c => c.Email = newEmail);

        private void ApplyChange(Action<ApplicationUser> changeAction)
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