using MapsterMapper;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EstablishmentSpecifications;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Establishments;

public class GetEstablishmentsHandler(
    IEstablishmentService establishmentService,
    IMapper mapper)
{
    private readonly IEstablishmentService _establishmentService = establishmentService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "EstablishmentId",
        string? sortOrder = "asc",
        string? establishmentId = null,
        string? establishmentName = null,
        string? establishmentEmail = null,
        int? establishmentStatusId = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(establishmentId))
        {
            return await GetEstablishmentByIdAsync(establishmentId, httpContext);
        }
        else
        {
            return await GetEstablishmentsListAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize }, httpContext, sortBy, sortOrder, establishmentName, establishmentEmail, establishmentStatusId);
        }
    }

    private async Task<IResult> GetEstablishmentByIdAsync(string establishmentId, HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<Establishment> result = await _establishmentService.GetEstablishmentByIdAsync(establishmentIdResult.Value!);

        return result.IsSuccess
            ? Results.Ok(_mapper.Map<EstablishmentResponse>(result.Value!))
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }

    private async Task<IResult> GetEstablishmentsListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? establishmentName,
        string? establishmentEmail,
        int? establishmentStatusId)
    {
        var spec = new EstablishmentSpecification(
            establishmentName: establishmentName,
            establishmentEmail: establishmentEmail,
            establishmentStatusId: establishmentStatusId,
            sortBy: sortBy,
            sortDirection: sortOrder,
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10
        );

        Result<int> countResult = await _establishmentService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<Establishment>> establishmentsResult = await _establishmentService.GetEstablishmentsAsync(spec);
        if (!establishmentsResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentsResult.ErrorType, establishmentsResult.Errors);
        }

        IEnumerable<Establishment> establishments = establishmentsResult.Value!;

        if (establishments == null || !establishments.Any())
        {
            return Results.NotFound();
        }

        IEnumerable<EstablishmentResponse> responseData = _mapper.Map<IEnumerable<EstablishmentResponse>>(establishments!);

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EstablishmentResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentName, establishmentEmail, establishmentStatusId),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentName, establishmentEmail, establishmentStatusId) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentName, establishmentEmail, establishmentStatusId) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentName, establishmentEmail, establishmentStatusId) : null
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
        string? establishmentName = null,
        string? establishmentEmail = null,
        int? establishmentStatusId = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(establishmentName))
        {
            link += $"&establishmentName={establishmentName}";
        }

        if (!string.IsNullOrEmpty(establishmentEmail))
        {
            link += $"&establishmentEmail={establishmentEmail}";
        }

        if (establishmentStatusId.HasValue)
        {
            link += $"&establishmentStatusId={establishmentStatusId}";
        }

        return link;
    }
}
