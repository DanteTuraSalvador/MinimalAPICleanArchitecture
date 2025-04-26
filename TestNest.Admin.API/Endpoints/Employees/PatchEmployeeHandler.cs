using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Responses;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Endpoints.Employees;

public class PatchEmployeeHandler(
    IEmployeeService employeeService,
    IMapper mapper,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string employeeId,
        [FromBody] JsonPatchDocument<EmployeePatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EmployeeId> employeeIdResult = IdHelper.ValidateAndCreateId<EmployeeId>(employeeId);
        if (!employeeIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeIdResult.ErrorType, employeeIdResult.Errors);
        }

        var employeePatchRequest = new EmployeePatchRequest();
        patchDocument.ApplyTo(employeePatchRequest);

        var validationContext = new ValidationContext(employeePatchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(employeePatchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<Employee> result = await _employeeService.PatchEmployeeAsync(employeeIdResult.Value!, employeePatchRequest);

        if (result.IsSuccess)
        {
            EmployeeResponse dto = _mapper.Map<EmployeeResponse>(result.Value!);
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
