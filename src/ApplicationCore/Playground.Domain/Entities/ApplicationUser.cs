namespace Playground.Domain.Entities
{
    using Microsoft.AspNetCore.Identity;
    using ValueObjects;

    public class ApplicationUser : IdentityUser
    {
        public Address Address { get; set; }
    }
}