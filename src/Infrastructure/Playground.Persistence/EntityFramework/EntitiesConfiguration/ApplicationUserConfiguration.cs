namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            //Address value object persisted as owned entity type supported since EF Core 2.0
            builder
                .OwnsOne(o => o.Address, a =>
                {
                    a.WithOwner();
                });
        }
    }
}