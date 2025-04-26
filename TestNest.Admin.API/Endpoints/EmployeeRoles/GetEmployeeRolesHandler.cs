using MapsterMapper;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EmployeeRoleSpecifications;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EmployeeRoles;

public class GetEmployeeRolesHandler(IEmployeeRoleService employeeRoleService, IMapper mapper)
{
    private readonly IEmployeeRoleService _employeeRoleService = employeeRoleService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "EmployeeRoleId",
        string? sortOrder = "asc",
        string? roleName = null,
        string? employeeRoleId = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(employeeRoleId))
        {
            return await GetEmployeeRoleByIdAsync(employeeRoleId, httpContext);
        }
        else
        {
            return await GetEmployeeRolesListAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize }, httpContext, sortBy, sortOrder, roleName);
        }
    }

    private async Task<IResult> GetEmployeeRoleByIdAsync(string employeeRoleId, HttpContext httpContext)
    {
        Result<EmployeeRoleId> employeeRoleIdValidatedResult = IdHelper
            .ValidateAndCreateId<EmployeeRoleId>(employeeRoleId);

        if (!employeeRoleIdValidatedResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeRoleIdValidatedResult.ErrorType, employeeRoleIdValidatedResult.Errors);
        }

        Result<EmployeeRole> result = await _employeeRoleService
            .GetEmployeeRoleByIdAsync(employeeRoleIdValidatedResult.Value!);

        return result.IsSuccess
            ? Results.Ok(_mapper.Map<EmployeeRoleResponse>(result.Value!))
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }


    private async Task<IResult> GetEmployeeRolesListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? roleName)
    {
        var spec = new EmployeeRoleSpecification(
            roleName: roleName,
            employeeRoleId: null,
            sortBy: sortBy,
            sortDirection: sortOrder,
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10
        );

        Result<int> countResult = await _employeeRoleService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<EmployeeRole>> employeeRolesResult = await _employeeRoleService.GetEmployeeRolessAsync(spec);
        if (!employeeRolesResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, employeeRolesResult.ErrorType, employeeRolesResult.Errors);
        }

        IEnumerable<EmployeeRole> employeeRoles = employeeRolesResult.Value!;

        if (employeeRoles == null || !employeeRoles.Any())
        {
            return Results.NotFound(); // Or handle with your ErrorResponseService
        }

        IEnumerable<EmployeeRoleResponse> responseData = _mapper.Map<IEnumerable<EmployeeRoleResponse>>(employeeRoles!);

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EmployeeRoleResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, roleName),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, roleName) : null, // Use ?? here
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, roleName) : null, // Use ?? here
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, roleName) : null // Use ?? here
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
        string? roleName = null)
    {
        string link = httpContext.Request.PathBase + httpContext.Request.Path + $"?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(roleName))
        {
            link += $"&roleName={roleName}";
        }
        return link;
    }
}
