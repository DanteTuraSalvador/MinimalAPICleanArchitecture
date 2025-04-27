using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Responses;

namespace TestNest.Admin.API.Endpoints.EmployeeRoles;

public class CreateEmployeeRoleHandler(
    IEmployeeRoleService employeeRoleService)
{
    private readonly IEmployeeRoleService _employeeRoleService = employeeRoleService;

    public async Task<IResult> HandleAsync(
        [FromBody] EmployeeRoleForCreationRequest employeeRoleForCreationRequest,
        HttpContext httpContext)
    {
        Result<EmployeeRoleResponse> result = await _employeeRoleService
            .CreateEmployeeRoleAsync(employeeRoleForCreationRequest);

        if (result.IsSuccess)
        {
            EmployeeRoleResponse dto = result.Value!;
            return Results.CreatedAtRoute("GetEmployeeRoles", new { employeeRoleId = dto.Id }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
