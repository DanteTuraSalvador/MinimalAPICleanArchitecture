using Microsoft.AspNetCore.JsonPatch;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace TestNest.Admin.API.Endpoints.Employees;

public static class EmployeeEndpoints
{
    public static void MapEmployeeApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        _ = group.MapPost("/", static async (
                [FromBody] EmployeeForCreationRequest request,
                HttpContext httpContext,
                CreateEmployeeHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEmployee")
            .Produces<EmployeeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new employee.")
            .WithDescription("Creates a new employee.");

        _ = group.MapPatch("/{employeeId}", static async (
                string employeeId,
                [FromBody] JsonPatchDocument<EmployeePatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEmployeeHandler handler) =>
            await handler.HandleAsync(employeeId, patchDocument, httpContext))
            .WithName("PatchEmployee")
            .Produces<EmployeeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing employee.")
            .WithDescription("Partially updates an existing employee.");

        _ = group.MapPut("/{employeeId}", static async (
                string employeeId,
                [FromBody] EmployeeForUpdateRequest request,
                HttpContext httpContext,
                UpdateEmployeeHandler handler) =>
            await handler.HandleAsync(employeeId, request, httpContext))
            .WithName("UpdateEmployee")
            .Produces<EmployeeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing employee.")
            .WithDescription("Updates an existing employee.");

        _ = group.MapDelete("/{employeeId}", static async (
                string employeeId,
                HttpContext httpContext,
                DeleteEmployeeHandler handler) =>
            await handler.HandleAsync(employeeId, httpContext))
            .WithName("DeleteEmployee")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an employee.")
            .WithDescription("Deletes an employee.");

        _ = group.MapGet("/", static async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEmployeesHandler handler,
                string? sortBy = "EmployeeId",
                string? sortOrder = "asc",
                string? employeeId = null,
                string? employeeNumber = null,
                string? firstName = null,
                string? middleName = null,
                string? lastName = null,
                string? emailAddress = null,
                int? employeeStatusId = null,
                string? employeeRoleId = null,
                string? establishmentId = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, employeeId, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId))
            .WithName("GetEmployees")
            .Produces<PaginatedResponse<EmployeeResponse>>(StatusCodes.Status200OK)
            .Produces<EmployeeResponse>(StatusCodes.Status200OK) // For GetById
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of employees or a single employee by ID.")
            .WithDescription("Retrieves a paginated list of employees with optional filtering, sorting, and pagination, or a single employee by ID.");
    }
}
