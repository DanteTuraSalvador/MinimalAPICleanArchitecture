using MapsterMapper;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.Application.Specifications.EmployeeSpecifications;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;

namespace TestNest.Admin.API.Endpoints.Employees;

public class GetEmployeesHandler(
    IEmployeeService employeeService,
    IMapper mapper,
    IErrorResponseService errorResponseService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
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
        string? establishmentId = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(employeeId))
        {
            return await GetEmployeeByIdAsync(employeeId, httpContext);
        }
        else
        {
            return await GetEmployeesListAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize }, httpContext, sortBy, sortOrder, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId);
        }
    }

    private async Task<IResult> GetEmployeeByIdAsync(string employeeId, HttpContext httpContext)
    {
        Result<EmployeeId> employeeIdResult = IdHelper.ValidateAndCreateId<EmployeeId>(employeeId);
        if (!employeeIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeIdResult.ErrorType, employeeIdResult.Errors);
        }

        Result<Employee> result = await _employeeService.GetEmployeeByIdAsync(employeeIdResult.Value!);

        return result.IsSuccess
            ? Results.Ok(_mapper.Map<EmployeeResponse>(result.Value!))
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }

    private async Task<IResult> GetEmployeesListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? employeeNumber,
        string? firstName,
        string? middleName,
        string? lastName,
        string? emailAddress,
        int? employeeStatusId,
        string? employeeRoleId,
        string? establishmentId)
    {
        var spec = new EmployeeSpecification(
            employeeNumber: employeeNumber,
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            emailAddress: emailAddress,
            employeeStatusId: employeeStatusId,
            employeeRoleId: employeeRoleId,
            establishmentId: establishmentId,
            sortBy: sortBy,
            sortDirection: sortOrder,
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10
        );

        Result<int> countResult = await _employeeService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<Employee>> employeesResult = await _employeeService.GetAllEmployeesAsync(spec);
        if (!employeesResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeesResult.ErrorType, employeesResult.Errors);
        }

        IEnumerable<Employee> employees = employeesResult.Value!;

        if (employees == null || !employees.Any())
        {
            return Results.NotFound();
        }

        IEnumerable<EmployeeResponse> responseData = _mapper.Map<IEnumerable<EmployeeResponse>>(employees!);

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EmployeeResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, employeeNumber, firstName, middleName, lastName, emailAddress, employeeStatusId, employeeRoleId, establishmentId) : null
            }
        };

        return Results.Ok(paginatedResponse);
    }

    private static string GeneratePaginationLink(
        HttpContext httpContext,
        int pageNumber,
        int pageSize,
        string sortBy,
        string sortOrder,
        string? employeeNumber = null,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        string? emailAddress = null,
        int? employeeStatusId = null,
        string? employeeRoleId = null,
        string? establishmentId = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(employeeNumber))
        {
            link += $"&employeeNumber={employeeNumber}";
        }

        if (!string.IsNullOrEmpty(firstName))
        {
            link += $"&firstName={firstName}";
        }

        if (!string.IsNullOrEmpty(middleName))
        {
            link += $"&middleName={middleName}";
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            link += $"&lastName={lastName}";
        }

        if (!string.IsNullOrEmpty(emailAddress))
        {
            link += $"&emailAddress={emailAddress}";
        }

        if (employeeStatusId.HasValue)
        {
            link += $"&employeeStatusId={employeeStatusId}";
        }

        if (!string.IsNullOrEmpty(employeeRoleId))
        {
            link += $"&employeeRoleId={employeeRoleId}";
        }

        if (!string.IsNullOrEmpty(establishmentId))
        {
            link += $"&establishmentId={establishmentId}";
        }

        return link;
    }
}
