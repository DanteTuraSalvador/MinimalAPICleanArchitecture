using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EstablishmentPhoneSpecifications;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public class GetEstablishmentPhonesHandler(
    IEstablishmentPhoneService establishmentPhoneService)
{
    private readonly IEstablishmentPhoneService _establishmentPhoneService = establishmentPhoneService;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "Id",
        string? sortOrder = "asc",
        string? establishmentId = null,
        string? establishmentPhoneId = null,
        string? phoneNumber = null,
        bool? isPrimary = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(establishmentPhoneId))
        {
            return await GetEstablishmentPhoneByIdAsync(establishmentPhoneId, httpContext);
        }
        else
        {
            return await GetEstablishmentPhonesListAsync(
                new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize },
                httpContext, sortBy, sortOrder, establishmentId, phoneNumber, isPrimary);
        }
    }

    private async Task<IResult> GetEstablishmentPhoneByIdAsync(string establishmentPhoneId, HttpContext httpContext)
    {
        Result<EstablishmentPhoneId> phoneIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentPhoneId>(establishmentPhoneId);
        if (!phoneIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, phoneIdResult.ErrorType, phoneIdResult.Errors);
        }

        Result<EstablishmentPhoneResponse> result = await _establishmentPhoneService
            .GetEstablishmentPhoneByIdAsync(phoneIdResult.Value!);

        return result.IsSuccess && result.Value != null
            ? Results.Ok(result.Value)
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", $"Establishment phone with ID '{establishmentPhoneId}' not found.")]);
    }

    private async Task<IResult> GetEstablishmentPhonesListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? establishmentId,
        string? phoneNumber,
        bool? isPrimary)
    {
        var spec = new EstablishmentPhoneSpecification(
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10,
            sortBy: sortBy,
            sortOrder: sortOrder,
            establishmentId: establishmentId,
            phoneNumber: phoneNumber,
            isPrimary: isPrimary
        );

        Result<int> countResult = await _establishmentPhoneService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<EstablishmentPhoneResponse>> phonesResult = await _establishmentPhoneService.ListAsync(spec);
        if (!phonesResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, phonesResult.ErrorType, phonesResult.Errors);
        }
        IEnumerable<EstablishmentPhoneResponse> phones = phonesResult.Value!;

        if (phones == null || !phones.Any())
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", "No establishment phones found matching the criteria.")]);
        }

        IEnumerable<EstablishmentPhoneResponse> responseData = phones;

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EstablishmentPhoneResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, phoneNumber, isPrimary),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, phoneNumber, isPrimary) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, phoneNumber, isPrimary) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, phoneNumber, isPrimary) : null
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
        string? establishmentId = null,
        string? establishmentPhoneId = null,
        string? phoneNumber = null,
        bool? isPrimary = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(establishmentId))
        {
            link += $"&establishmentId={establishmentId}";
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            link += $"&phoneNumber={phoneNumber}";
        }

        if (isPrimary.HasValue)
        {
            link += $"&isPrimary={isPrimary}";
        }

        return link;
    }
}
