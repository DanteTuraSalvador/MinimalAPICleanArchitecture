using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EstablishmentMemberConfiguration : IEntityTypeConfiguration<EstablishmentMember>
{
    public void Configure(EntityTypeBuilder<EstablishmentMember> builder)
    {
        builder.ToTable("EstablishmentMembers");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EstablishmentMemberId>()
            .HasColumnName("EstablishmentMemberId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EmployeeId)
            .ConfigureStronglyTypedId<EmployeeId>()
            .HasColumnName("EmployeeId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.MemberTitle, nameBuilder =>
        {
            nameBuilder.Property(n => n.Title)
                .HasColumnName("MemberTitle")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(e => e.MemberDescription, descriptionBuilder =>
        {
            descriptionBuilder.Property(d => d.Description)
                .HasColumnName("MemberDescription")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.OwnsOne(e => e.MemberTag, tagBuilder =>
        {
            tagBuilder.Property(t => t.Tag)
                .HasColumnName("MemberTag")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.HasOne(e => e.Establishment)
            .WithMany()
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}