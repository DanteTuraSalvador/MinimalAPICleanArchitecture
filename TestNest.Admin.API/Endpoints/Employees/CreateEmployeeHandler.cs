using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Endpoints.Employees;

public class CreateEmployeeHandler(
    IEmployeeService employeeService,
    IMapper mapper,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] EmployeeForCreationRequest request,
        HttpContext httpContext)
    {
        Result<Employee> result = await _employeeService.CreateEmployeeAsync(request);

        if (result.IsSuccess)
        {
            EmployeeResponse dto = _mapper.Map<EmployeeResponse>(result.Value!);
            return Results.CreatedAtRoute("GetEmployees", new { employeeId = dto.EmployeeId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
