using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EstablishmentAddressesSpecifications;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public class GetEstablishmentAddressesHandler(
    IEstablishmentAddressService establishmentAddressService)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "Id",
        string? sortOrder = "asc",
        string? establishmentId = null,
        string? establishmentAddressId = null,
        string? city = null,
        string? municipality = null,
        string? province = null,
        string? region = null,
        bool? isPrimary = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(establishmentAddressId))
        {
            return await GetEstablishmentAddressByIdAsync(establishmentAddressId, httpContext);
        }
        else
        {
            return await GetEstablishmentAddressesListAsync(
                new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize },
                httpContext, sortBy, sortOrder, establishmentId, city, municipality, province, region, isPrimary);
        }
    }

    private async Task<IResult> GetEstablishmentAddressByIdAsync(string establishmentAddressId, HttpContext httpContext)
    {
        Result<EstablishmentAddressId> addressIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentAddressId>(establishmentAddressId);
        if (!addressIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, addressIdResult.ErrorType, addressIdResult.Errors);
        }

        Result<EstablishmentAddressResponse> result = await _establishmentAddressService
            .GetEstablishmentAddressByIdAsync(addressIdResult.Value!);

        return result.IsSuccess && result.Value != null
            ? Results.Ok(result.Value)
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", $"Establishment address with ID '{establishmentAddressId}' not found.")]);
    }

    private async Task<IResult> GetEstablishmentAddressesListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? establishmentId,
        string? city,
        string? municipality,
        string? province,
        string? region,
        bool? isPrimary)
    {
        var spec = new EstablishmentAddressSpecification(
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10,
            sortBy: sortBy,
            sortOrder: sortOrder,
            establishmentId: establishmentId,
            city: city,
            municipality: municipality,
            province: province,
            region: region,
            isPrimary: isPrimary
        );

        Result<int> countResult = await _establishmentAddressService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<EstablishmentAddressResponse>> addressesResult = await _establishmentAddressService.ListAsync(spec);
        if (!addressesResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, addressesResult.ErrorType, addressesResult.Errors);
        }
        IEnumerable<EstablishmentAddressResponse> addresses = addressesResult.Value!;

        if (addresses == null || !addresses.Any())
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", "No establishment addresses found matching the criteria.")]);
        }

        IEnumerable<EstablishmentAddressResponse> responseData = addresses;

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EstablishmentAddressResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, city, municipality, province, region, isPrimary),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, city, municipality, province, region, isPrimary) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, city, municipality, province, region, isPrimary) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, city, municipality, province, region, isPrimary) : null
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
        string? establishmentAddressId = null,
        string? city = null,
        string? municipality = null,
        string? province = null,
        string? region = null,
        bool? isPrimary = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(establishmentId))
        {
            link += $"&establishmentId={establishmentId}";
        }

        if (!string.IsNullOrEmpty(city))
        {
            link += $"&city={city}";
        }

        if (!string.IsNullOrEmpty(municipality))
        {
            link += $"&municipality={municipality}";
        }

        if (!string.IsNullOrEmpty(province))
        {
            link += $"&province={province}";
        }

        if (!string.IsNullOrEmpty(region))
        {
            link += $"&region={region}";
        }

        if (isPrimary.HasValue)
        {
            link += $"&isPrimary={isPrimary}";
        }

        return link;
    }
}
