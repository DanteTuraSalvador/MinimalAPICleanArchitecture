using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Infrastructure.Persistence;
using TestNest.Admin.Infrastructure.Persistence.Repositories;
using TestNest.Admin.Infrastructure.Persistence.UnitOfWork;

namespace TestNest.Admin.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        _ = services.AddScoped<ISocialMediaPlatformRepository, SocialMediaPlatformRepository>();
        _ = services.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
        _ = services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
        _ = services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        _ = services.AddScoped<IEstablishmentAddressRepository, EstablishmentAddressRepository>();
        _ = services.AddScoped<IEstablishmentContactRepository, EstablishmentContactRepository>();
        _ = services.AddScoped<IEstablishmentPhoneRepository, EstablishmentPhoneRepository>();
        _ = services.AddScoped<IEstablishmentMemberRepository, EstablishmentMemberRepository>();
        _ = services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
