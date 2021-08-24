namespace Playground.Domain.Entities.Aggregates.User
{
    using Abstract;
    using Exceptions;
    using Microsoft.AspNetCore.Identity;
    using System;

    public class ApplicationUser : IdentityUser, IAggregateRoot
    {
        // constructor for ef
        protected ApplicationUser() { }

        public ApplicationUser(UserId userId, Email email)
        {
            Id = userId.ToString();
            Email = email;
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