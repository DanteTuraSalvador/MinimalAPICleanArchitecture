using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentSocialMediaConfiguration : IEntityTypeConfiguration<EstablishmentSocialMedia>
{
    public void Configure(EntityTypeBuilder<EstablishmentSocialMedia> builder)
    {
        builder.ToTable("EstablishmentSocialMedia");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentSocialMediaId>()
            .HasColumnName("EstablishmentSocialMediaId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.SocialMediaId)
            .ConfigureStronglyTypedId<SocialMediaId>()
            .HasColumnName("SocialMediaId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.SocialMediaAccountName, nameBuilder =>
        {
            nameBuilder.Property(n => n.AccountName)
                .HasColumnName("SocialMediaAccountName")
                .HasColumnType("VARCHAR")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.HasOne(e => e.Establishment)
            .WithMany()
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        ;

        builder.HasOne(e => e.SocialMedia)
            .WithMany()
            .HasForeignKey(e => e.SocialMediaId)
            .OnDelete(DeleteBehavior.Cascade);
        ;
    }
}
