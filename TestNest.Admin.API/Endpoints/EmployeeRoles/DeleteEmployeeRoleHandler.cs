using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EmployeeRoles;

public class DeleteEmployeeRoleHandler(IEmployeeRoleService employeeRoleService)
{
    private readonly IEmployeeRoleService _employeeRoleService = employeeRoleService;

    public async Task<IResult> HandleAsync(string employeeRoleId, HttpContext httpContext)
    {
        Result<EmployeeRoleId> employeeRoleIdValidatedResult = IdHelper
            .ValidateAndCreateId<EmployeeRoleId>(employeeRoleId);

        if (!employeeRoleIdValidatedResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, employeeRoleIdValidatedResult.Errors);
        }

        Result result = await _employeeRoleService
            .DeleteEmployeeRoleAsync(employeeRoleIdValidatedResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
