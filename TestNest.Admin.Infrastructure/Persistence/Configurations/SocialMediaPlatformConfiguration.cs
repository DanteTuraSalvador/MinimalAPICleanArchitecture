using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class SocialMediaPlatformConfiguration : IEntityTypeConfiguration<SocialMediaPlatform>
{
    public void Configure(EntityTypeBuilder<SocialMediaPlatform> builder)
    {
        builder.ToTable("SocialMediaPlatforms");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<SocialMediaId>()
            .HasColumnName("SocialMediaId")
             .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.SocialMediaName, nameBuilder =>
        {
            nameBuilder.HasIndex(n => n.Name)
                .IsUnique()
                .HasDatabaseName("IX_SocialMediaName");

            nameBuilder.Property(n => n.Name)
                .HasColumnName("SocialMediaName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(50)
                .IsRequired();

            nameBuilder.Property(n => n.PlatformURL)
                .HasColumnName("PlatformURL")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}