using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentContactConfiguration : IEntityTypeConfiguration<EstablishmentContact>
{
    public void Configure(EntityTypeBuilder<EstablishmentContact> builder)
    {
        builder.ToTable("EstablishmentContacts");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentContactId>()
            .HasColumnName("EstablishmentContactId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.ContactPerson, nameBuilder =>
        {
            nameBuilder.Property(n => n.FirstName)
                .HasColumnName("ContactFirstName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(n => n.MiddleName)
                .HasColumnName("ContactMiddleName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(n => n.LastName)
                .HasColumnName("ContactLastName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(e => e.ContactPhone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.PhoneNo)
                .HasColumnName("ContactPhone")
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
            .HasForeignKey(e => e.EstablishmentId).OnDelete(DeleteBehavior.NoAction);
        ;
    }
}
