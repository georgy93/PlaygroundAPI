namespace Playground.Persistence.EntityFramework.Extensions
{
    using Domain.SeedWork;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class EntityBuilderExtensions
    {
        public static void ConfigureEnumeration<TEnum>(this EntityTypeBuilder<TEnum> builder) where TEnum : Enumeration
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName(nameof(Enumeration.Id))
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(o => o.Name)
                 .HasColumnName(nameof(Enumeration.Name))
                 .HasMaxLength(255)
                 .IsRequired();
        }
    }
}