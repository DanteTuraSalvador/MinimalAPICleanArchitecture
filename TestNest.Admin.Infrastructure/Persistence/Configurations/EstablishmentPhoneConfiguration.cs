using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentPhoneConfiguration : IEntityTypeConfiguration<EstablishmentPhone>
{
    public void Configure(EntityTypeBuilder<EstablishmentPhone> builder)
    {
        builder.ToTable("EstablishmentPhones");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentPhoneId>()
            .HasColumnName("EstablishmentPhoneId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.EstablishmentPhoneNumber, nameBuilder =>
        {
            nameBuilder.Property(n => n.PhoneNo)
                .HasColumnName("PhoneNumber")
                .HasColumnType("VARCHAR")
                .HasMaxLength(15)
                .IsRequired();
        });

        builder.Property(e => e.IsPrimary)
            .HasColumnName("IsPrimary")
            .HasColumnType("BIT")
            .HasDefaultValue(1)
            .IsRequired();

        builder.HasOne(e => e.Establishment)
            .WithMany()
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}