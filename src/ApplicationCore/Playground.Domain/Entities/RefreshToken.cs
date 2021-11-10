namespace Playground.Domain.Entities
{
    using SeedWork;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RefreshToken : Entity<int>
    {
        internal RefreshToken() { }

        public override int Id { get; protected set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; private set; }

        public string JwtId { get; init; }

        //[Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; init; }

        public DateTime ExpiryDate { get; init; }

        public bool Used { get; private set; }

        public bool Invalidated { get; private set; }

        public string UserId { get; init; } // NO Direct reference to User!

        public void SetInUse() => Used = true;

        public static RefreshToken New(string tokenId, string userId, DateTime creationDate, DateTime expiryDate) => new()
        {
            JwtId = tokenId,
            UserId = userId,
            CreationDate = creationDate,
            ExpiryDate = expiryDate
        };
    }
}