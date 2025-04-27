using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Employees;

public class UpdateEmployeeHandler(
    IEmployeeService employeeService,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResult> HandleAsync(
        string employeeId,
        [FromBody] EmployeeForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EmployeeId> employeeIdResult = IdHelper.ValidateAndCreateId<EmployeeId>(employeeId);
        if (!employeeIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeIdResult.ErrorType, employeeIdResult.Errors);
        }

        Result<EmployeeResponse> result = await _employeeService.UpdateEmployeeAsync(employeeIdResult.Value!, request);

        if (result.IsSuccess)
        {
            EmployeeResponse dto = result.Value!;
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
