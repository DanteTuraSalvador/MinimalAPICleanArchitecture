using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentConfiguration : IEntityTypeConfiguration<Establishment>
{
    public void Configure(EntityTypeBuilder<Establishment> builder)
    {
        _ = builder.ToTable("Establishments");

        _ = builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.EstablishmentName, nameBuilder =>
        {
            nameBuilder.Property(n => n.Name)
                .HasColumnName("EstablishmentName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(e => e.EstablishmentEmail, emailBuilder =>
        {
            emailBuilder.Property(e => e.Email)
                .HasColumnName("EstablishmentEmail")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(e => e.EstablishmentStatus)
            .HasConversion(
            status => status.Id,
            value => EstablishmentStatus.FromIdOrDefault(value))
            .HasColumnName("EstablishmentStatusId")
            .HasColumnType("INT")
            .HasDefaultValueSql("(-1)")
            .IsRequired();
    }
}
