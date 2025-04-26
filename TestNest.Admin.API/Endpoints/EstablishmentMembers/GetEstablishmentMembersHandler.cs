using MapsterMapper;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EstablishmentMemberSpecifications;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public class GetEstablishmentMembersHandler(
    IEstablishmentMemberService establishmentMemberService,
    IMapper mapper)
{
    private readonly IEstablishmentMemberService _establishmentMemberService = establishmentMemberService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "Id",
        string? sortOrder = "asc",
        string? establishmentMemberId = null,
        string? establishmentId = null,
        string? employeeId = null,
        string? memberTitle = null,
        string? memberDescription = null,
        string? memberTag = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(establishmentMemberId))
        {
            return await GetEstablishmentMemberByIdAsync(establishmentMemberId, httpContext);
        }
        else
        {
            return await GetEstablishmentMembersListAsync(
                new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize },
                httpContext, sortBy, sortOrder, establishmentId, employeeId, memberTitle, memberDescription, memberTag);
        }
    }

    private async Task<IResult> GetEstablishmentMemberByIdAsync(string establishmentMemberId, HttpContext httpContext)
    {
        Result<EstablishmentMemberId> memberIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentMemberId>(establishmentMemberId);
        if (!memberIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, memberIdResult.ErrorType, memberIdResult.Errors);
        }

        Result<EstablishmentMember> result = await _establishmentMemberService
            .GetEstablishmentMemberByIdAsync(memberIdResult.Value!);

        return result.IsSuccess && result.Value != null
            ? Results.Ok(_mapper.Map<EstablishmentMemberResponse>(result.Value))
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", $"Establishment member with ID '{establishmentMemberId}' not found.")]);
    }

    private async Task<IResult> GetEstablishmentMembersListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? establishmentId,
        string? employeeId,
        string? memberTitle,
        string? memberDescription,
        string? memberTag)
    {
        var spec = new EstablishmentMemberSpecification(
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10,
            sortBy: sortBy,
            sortOrder: sortOrder,
            establishmentId: establishmentId,
            employeeId: employeeId,
            memberTitle: memberTitle,
            memberDescription: memberDescription,
            memberTag: memberTag
        );

        Result<int> countResult = await _establishmentMemberService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<EstablishmentMember>> membersResult = await _establishmentMemberService.ListAsync(spec);
        if (!membersResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, membersResult.ErrorType, membersResult.Errors);
        }
        IEnumerable<EstablishmentMember> members = membersResult.Value!;

        if (members == null || !members.Any())
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", "No establishment members found matching the criteria.")]);
        }

        IEnumerable<EstablishmentMemberResponse> responseData = _mapper.Map<IEnumerable<EstablishmentMemberResponse>>(members);

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EstablishmentMemberResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, null, establishmentId, employeeId, memberTitle, memberDescription, memberTag),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, null, establishmentId, employeeId, memberTitle, memberDescription, memberTag) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, null, establishmentId, employeeId, memberTitle, memberDescription, memberTag) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, null, establishmentId, employeeId, memberTitle, memberDescription, memberTag) : null
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
        string? establishmentMemberId = null,
        string? establishmentId = null,
        string? employeeId = null,
        string? memberTitle = null,
        string? memberDescription = null,
        string? memberTag = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(establishmentMemberId))
        {
            link += $"&establishmentMemberId={establishmentMemberId}";
        }

        if (!string.IsNullOrEmpty(establishmentId))
        {
            link += $"&establishmentId={establishmentId}";
        }

        if (!string.IsNullOrEmpty(employeeId))
        {
            link += $"&employeeId={employeeId}";
        }

        if (!string.IsNullOrEmpty(memberTitle))
        {
            link += $"&memberTitle={memberTitle}";
        }

        if (!string.IsNullOrEmpty(memberDescription))
        {
            link += $"&memberDescription={memberDescription}";
        }

        if (!string.IsNullOrEmpty(memberTag))
        {
            link += $"&memberTag={memberTag}";
        }

        return link;
    }
}
