using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Employees;

public class DeleteEmployeeHandler(
    IEmployeeService employeeService,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResult> HandleAsync(string employeeId, HttpContext httpContext)
    {
        Result<EmployeeId> employeeIdResult = IdHelper.ValidateAndCreateId<EmployeeId>(employeeId);
        if (!employeeIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeIdResult.ErrorType, employeeIdResult.Errors);
        }

        Result result = await _employeeService.DeleteEmployeeAsync(employeeIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
