namespace Playground.Persistence.EntityFramework.Extensions;

using Ardalis.SmartEnum;

public static class EntityBuilderExtensions
{
    extension<TEnum>(EntityTypeBuilder<TEnum> builder) where TEnum : SmartEnum<TEnum>
    {
        public void ConfigureEnumeration()
        {
            builder.HasKey(o => o.Value);

            builder.Property(o => o.Value)
                .HasColumnName("Id")
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(o => o.Name)
                 .HasColumnName("Name")
                 .HasMaxLength(255)
                 .IsRequired();
        }
    }
}