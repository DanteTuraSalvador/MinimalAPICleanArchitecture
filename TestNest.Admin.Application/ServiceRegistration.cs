using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Exceptions;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Services;

namespace TestNest.Admin.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped<IDatabaseExceptionHandlerFactory, SqlServerExceptionHandlerFactory>();
        _ = services.AddScoped<ISocialMediaPlatformService, SocialMediaPlatformService>();
        _ = services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
        _ = services.AddScoped<IEstablishmentService, EstablishmentService>();
        _ = services.AddScoped<IEmployeeService, EmployeeService>();
        _ = services.AddScoped<IEstablishmentAddressService, EstablishmentAddressService>();
        _ = services.AddScoped<IEstablishmentContactService, EstablishmentContactService>();
        _ = services.AddScoped<IEstablishmentPhoneService, EstablishmentPhoneService>();
        _ = services.AddScoped<IEstablishmentMemberService, EstablishmentMemberService>();
        return services;
    }
}
