using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Domain.SocialMedias;

namespace TestNest.Admin.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SocialMediaPlatform> SocialMediaPlatforms { get; set; }
    public DbSet<EmployeeRole> EmployeeRoles { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<EstablishmentAddress> EstablishmentAddresses { get; set; }
    public DbSet<EstablishmentContact> EstablishmentContacts { get; set; }
    public DbSet<EstablishmentPhone> EstablishmentPhones { get; set; }
    public DbSet<EstablishmentMember> EstablishmentMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
