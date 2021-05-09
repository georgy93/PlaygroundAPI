namespace Playground.Domain.Entities
{
    using Abstract;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RefreshToken : Entity<int>
    {
        public override int Id { get; protected set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }

        public string JwtId { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool Used { get; set; }

        public bool Invalidated { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; } // TODO no direct link, use just UserId
    }
}
