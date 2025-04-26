using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.Infrastructure.Persistence.Configurations.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id)
            .IsClustered(false);

        builder.Property(e => e.Id)
            .ConfigureStronglyTypedId<EmployeeId>()
            .HasColumnName("EmployeeId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.OwnsOne(e => e.EmployeeNumber, employeeNumberBuilder =>
        {
            employeeNumberBuilder.Property(e => e.EmployeeNo)
                .HasColumnName("EmployeeNumber")
                .HasColumnType("VARCHAR")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.OwnsOne(e => e.EmployeeName, nameBuilder =>
        {
            nameBuilder.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(n => n.MiddleName)
                .HasColumnName("MiddleName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100);

            nameBuilder.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(e => e.EmployeeEmail, emailBuilder =>
        {
            emailBuilder.Property(e => e.Email)
                .HasColumnName("EmailAddress")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(e => e.EmployeeStatus)
          .HasConversion(
              status => status.Id,
              value => EmployeeStatus.FromIdOrDefault(value)
          )
          .HasColumnName("EmployeeStatusId")
          .HasColumnType("INT")
          .HasDefaultValueSql("(-1)")
          .IsRequired();

        builder.Property(e => e.EmployeeRoleId)
            .ConfigureStronglyTypedId<EmployeeRoleId>()
            .HasColumnName("EmployeeRoleId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(e => e.EstablishmentId)
            .ConfigureStronglyTypedId<EstablishmentId>()
            .HasColumnName("EstablishmentId")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.HasOne(e => e.Establishment)
            .WithMany()
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.EmployeeRole)
            .WithMany()
            .HasForeignKey(e => e.EmployeeRoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
