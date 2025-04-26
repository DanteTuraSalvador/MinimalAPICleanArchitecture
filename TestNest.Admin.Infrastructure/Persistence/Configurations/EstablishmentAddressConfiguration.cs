using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentAddressConfiguration : IEntityTypeConfiguration<EstablishmentAddress>
{
    public void Configure(EntityTypeBuilder<EstablishmentAddress> builder)
    {
        _ = builder.ToTable("EstablishmentAddresses");

        _ = builder.HasKey(ea => ea.Id)
            .IsClustered(false);

        _ = builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentAddressId>()
            .HasColumnName("EstablishmentAddressId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        _ = builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        _ = builder.OwnsOne(ea => ea.Address, addressBuilder =>
        {
            _ = addressBuilder.Property(a => a.AddressLine)
                .HasColumnName("AddressLine")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(255)
                .IsRequired();

            _ = addressBuilder.Property(a => a.City)
                .HasColumnName("City")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Municipality)
                .HasColumnName("Municipality")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Province)
                .HasColumnName("Province")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Region)
                .HasColumnName("Region")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Country)
                .HasColumnName("Country")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Latitude)
                .HasColumnName("Latitude")
                .HasColumnType("FLOAT")
                .IsRequired();

            addressBuilder.Property(a => a.Longitude)
                .HasColumnName("Longitude")
                .HasColumnType("FLOAT")
                .IsRequired();
        });

        builder.Property(e => e.IsPrimary)
            .HasColumnName("IsPrimary")
            .HasColumnType("BIT")
            .HasDefaultValue(1)
            .IsRequired();

        // Relationship with Establishment (if needed)
        builder.HasOne(e => e.Establishment)
            .WithMany()
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Relationship with Establishment
        //builder.HasOne(e => e.Establishment)
        //    .WithMany(e => e.Addresses) // Assuming Establishment has a collection
        //    .HasForeignKey(e => e.EstablishmentId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
