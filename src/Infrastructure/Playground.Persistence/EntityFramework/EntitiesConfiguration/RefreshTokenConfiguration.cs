namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities;

    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Token);
        }
    }
}