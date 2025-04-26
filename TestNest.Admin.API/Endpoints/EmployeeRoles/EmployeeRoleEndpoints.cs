using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;


namespace TestNest.Admin.API.Endpoints.EmployeeRoles;

public static class EmployeeRoleEndpoints
{
    public static void MapEmployeeRoleApi(this WebApplication app)
    {
        RouteGroupBuilder employeeRoleGroup = app.MapGroup("/api/employeeroles")
            .WithTags("EmployeeRoles");

        _ = employeeRoleGroup.MapPost("/", async (
                [FromBody] EmployeeRoleForCreationRequest employeeRoleForCreationRequest,
                HttpContext httpContext,
                CreateEmployeeRoleHandler handler) => await handler.HandleAsync(employeeRoleForCreationRequest, httpContext))
            .WithName("CreateEmployeeRole")
            .Produces<EmployeeRoleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new employee role.")
            .WithDescription("Creates a new employee role.");

        _ = employeeRoleGroup.MapPut("/{employeeRoleId}", async (
                string employeeRoleId,
                [FromBody] EmployeeRoleForUpdateRequest employeeRoleForUpdateRequest,
                HttpContext httpContext,
                UpdateEmployeeRoleHandler handler) => await handler.HandleAsync(employeeRoleId, employeeRoleForUpdateRequest, httpContext))
            .WithName("UpdateEmployeeRole")
            .Produces<EmployeeRoleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing employee role.")
            .WithDescription("Updates an existing employee role.");

        _ = employeeRoleGroup.MapDelete("/{employeeRoleId}", async (
                string employeeRoleId,
                HttpContext httpContext,
                DeleteEmployeeRoleHandler handler) => await handler.HandleAsync(employeeRoleId, httpContext))
            .WithName("DeleteEmployeeRole")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an employee role.")
            .WithDescription("Deletes an employee role.");

        _ = employeeRoleGroup.MapGet("/", async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEmployeeRolesHandler handler,
                string? sortBy = "EmployeeRoleId",
                string? sortOrder = "asc",
                string? roleName = null,
                string? employeeRoleId = null) => await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, roleName, employeeRoleId))
            .WithName("GetEmployeeRoles")
            .Produces<PaginatedResponse<EmployeeRoleResponse>>(StatusCodes.Status200OK)
            .Produces<EmployeeRoleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets a list of employee roles or a single employee role by ID.")
            .WithDescription("Gets a list of employee roles with optional filtering, sorting, and pagination, or a single employee role by ID.");


    }
}

