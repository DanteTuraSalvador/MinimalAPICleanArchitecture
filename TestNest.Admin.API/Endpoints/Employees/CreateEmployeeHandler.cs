using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Endpoints.Employees;

public class CreateEmployeeHandler(
    IEmployeeService employeeService,

    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResult> HandleAsync(
        [FromBody] EmployeeForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EmployeeResponse> result = await _employeeService.CreateEmployeeAsync(request);

        if (result.IsSuccess)
        {
            EmployeeResponse dto = result.Value!;
            return Results.CreatedAtRoute("GetEmployees", new { employeeId = dto.EmployeeId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
