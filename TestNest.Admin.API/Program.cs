using System.Reflection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using TestNest.Admin.API.Endpoints.EmployeeRoles;
using TestNest.Admin.API.Endpoints.Employees;
using TestNest.Admin.API.Endpoints.EstablishmentAddresses;
using TestNest.Admin.API.Endpoints.EstablishmentContacts;
using TestNest.Admin.API.Endpoints.EstablishmentMembers;
using TestNest.Admin.API.Endpoints.EstablishmentPhones;
using TestNest.Admin.API.Endpoints.Establishments;
using TestNest.Admin.API.Endpoints.SocialMediaPlatforms;
using TestNest.Admin.API.Exceptions;
using TestNest.Admin.API.Middleware;
using TestNest.Admin.Application;
using TestNest.Admin.Infrastructure;
using TestNest.Admin.Infrastructure.Persistence;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllers()
            .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

        _ = builder.Services.AddEndpointsApiExplorer();

        var openApiPatchSchema = new OpenApiSchema
        {
            Type = "array",
            Items = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "op", new OpenApiSchema { Type = "string", Description = "Operation type (replace, add, remove, etc.)" } },
                    { "path", new OpenApiSchema { Type = "string", Description = "Path to the property to update" } },
                    { "value", new OpenApiSchema { Type = "object", Description = "New value for the property" } }
                }
            }
        };

        _ = builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Palawan Nest", Version = "v1" });
            c.MapType<JsonPatchDocument<EmployeePatchRequest>>(() => openApiPatchSchema);
            c.MapType<JsonPatchDocument<EstablishmentPatchRequest>>(() => openApiPatchSchema);
            c.MapType<JsonPatchDocument<EstablishmentAddressPatchRequest>>(() => openApiPatchSchema);
            c.MapType<JsonPatchDocument<EstablishmentContactPatchRequest>>(() => openApiPatchSchema);
            c.MapType<JsonPatchDocument<EstablishmentMemberPatchRequest>>(() => openApiPatchSchema);
            c.MapType<JsonPatchDocument<EstablishmentPhonePatchRequest>>(() => openApiPatchSchema);

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        _ = builder.Services.AddPersistenceServices(builder.Configuration);
        _ = builder.Services.AddScoped<IErrorResponseService, ErrorResponseService>();
        _ = builder.Services.AddApplicationServices(builder.Configuration);

        _ = builder.Services.AddHttpContextAccessor();
        _ = builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(
                 builder.Configuration.GetConnectionString("DefaultConnection"),
                 sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
             )
         );

        // Add the handler registrations here:
        _ = builder.Services.AddScoped<CreateEmployeeRoleHandler>();
        _ = builder.Services.AddScoped<UpdateEmployeeRoleHandler>();
        _ = builder.Services.AddScoped<DeleteEmployeeRoleHandler>();
        _ = builder.Services.AddScoped<GetEmployeeRolesHandler>();

        _ = builder.Services.AddScoped<CreateSocialMediaPlatformHandler>();
        _ = builder.Services.AddScoped<UpdateSocialMediaPlatformHandler>();
        _ = builder.Services.AddScoped<DeleteSocialMediaPlatformHandler>();
        _ = builder.Services.AddScoped<GetSocialMediaPlatformsHandler>();

        _ = builder.Services.AddScoped<CreateEmployeeHandler>();
        _ = builder.Services.AddScoped<UpdateEmployeeHandler>();
        _ = builder.Services.AddScoped<PatchEmployeeHandler>();
        _ = builder.Services.AddScoped<DeleteEmployeeHandler>();
        _ = builder.Services.AddScoped<GetEmployeesHandler>();

        _ = builder.Services.AddScoped<CreateEstablishmentHandler>();
        _ = builder.Services.AddScoped<UpdateEstablishmentHandler>();
        _ = builder.Services.AddScoped<PatchEstablishmentHandler>();
        _ = builder.Services.AddScoped<DeleteEstablishmentHandler>();
        _ = builder.Services.AddScoped<GetEstablishmentsHandler>();

        _ = builder.Services.AddScoped<CreateEstablishmentAddressHandler>();
        _ = builder.Services.AddScoped<UpdateEstablishmentAddressHandler>();
        _ = builder.Services.AddScoped<DeleteEstablishmentAddressHandler>();
        _ = builder.Services.AddScoped<GetEstablishmentAddressesHandler>();
        _ = builder.Services.AddScoped<PatchEstablishmentAddressHandler>();

        _ = builder.Services.AddScoped<CreateEstablishmentContactHandler>();
        _ = builder.Services.AddScoped<UpdateEstablishmentContactHandler>();
        _ = builder.Services.AddScoped<PatchEstablishmentContactHandler>();
        _ = builder.Services.AddScoped<DeleteEstablishmentContactHandler>();
        _ = builder.Services.AddScoped<GetEstablishmentContactsHandler>();

        _ = builder.Services.AddScoped<CreateEstablishmentMemberHandler>();
        _ = builder.Services.AddScoped<UpdateEstablishmentMemberHandler>();
        _ = builder.Services.AddScoped<PatchEstablishmentMemberHandler>();
        _ = builder.Services.AddScoped<DeleteEstablishmentMemberHandler>();
        _ = builder.Services.AddScoped<GetEstablishmentMembersHandler>();

        _ = builder.Services.AddScoped<CreateEstablishmentPhoneHandler>();
        _ = builder.Services.AddScoped<UpdateEstablishmentPhoneHandler>();
        _ = builder.Services.AddScoped<PatchEstablishmentPhoneHandler>();
        _ = builder.Services.AddScoped<DeleteEstablishmentPhoneHandler>();
        _ = builder.Services.AddScoped<GetEstablishmentPhonesHandler>();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseMiddleware<ExceptionHandlingMiddleware>();
        _ = app.UseAuthorization();

        app.MapEmployeeRoleApi();
        app.MapSocialMediaPlatformApi();
        app.MapEmployeeApi();
        app.MapEstablishmentApi();
        app.MapEstablishmentAddressApi();
        app.MapEstablishmentContactApi();
        app.MapEstablishmentMemberApi();
        app.MapEstablishmentPhoneApi();

        _ = app.MapControllers();
        app.Run();
    }
}
