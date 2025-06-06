﻿using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Responses;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EmployeeRoles;

public class UpdateEmployeeRoleHandler(IEmployeeRoleService employeeRoleService, IErrorResponseService errorResponseService)
{
    private readonly IEmployeeRoleService _employeeRoleService = employeeRoleService;
    private readonly IErrorResponseService _errorResponseService = errorResponseService;

    public async Task<IResult> HandleAsync(
        string employeeRoleId,
        [FromBody] EmployeeRoleForUpdateRequest employeeRoleForUpdateRequest,
        HttpContext httpContext)
    {
        Result<EmployeeRoleId> employeeRoleIdValidatedResult = IdHelper
            .ValidateAndCreateId<EmployeeRoleId>(employeeRoleId);

        if (!employeeRoleIdValidatedResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeRoleIdValidatedResult.ErrorType, employeeRoleIdValidatedResult.Errors);
        }

        Result<EmployeeRoleResponse> updatedEmployeeRole = await _employeeRoleService
            .UpdateEmployeeRoleAsync(
                employeeRoleIdValidatedResult.Value!,
                employeeRoleForUpdateRequest);

        if (updatedEmployeeRole.IsSuccess)
        {
            return Results.Ok(updatedEmployeeRole.Value!);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, updatedEmployeeRole.ErrorType, updatedEmployeeRole.Errors);
    }
}
