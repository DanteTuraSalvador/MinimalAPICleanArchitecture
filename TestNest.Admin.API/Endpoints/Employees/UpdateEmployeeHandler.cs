using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Employees;

public class UpdateEmployeeHandler(
    IEmployeeService employeeService,
    IMapper mapper,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IMapper _mapper = mapper;

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

        Result<Employee> result = await _employeeService.UpdateEmployeeAsync(employeeIdResult.Value!, request);

        if (result.IsSuccess)
        {
            EmployeeResponse dto = _mapper.Map<EmployeeResponse>(result.Value!);
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
