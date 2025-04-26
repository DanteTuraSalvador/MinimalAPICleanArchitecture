using MapsterMapper;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.EstablishmentContactSpecifications;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentContacts;

public class GetEstablishmentContactsHandler(
    IEstablishmentContactService establishmentContactService,
    IMapper mapper)
{
    private readonly IEstablishmentContactService _establishmentContactService = establishmentContactService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "Id",
        string? sortOrder = "asc",
        string? establishmentId = null,
        string? establishmentContactId = null,
        string? contactPersonFirstName = null,
        string? contactPersonMiddleName = null,
        string? contactPersonLastName = null,
        string? contactPhoneNumber = null,
        bool? isPrimary = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(establishmentContactId))
        {
            return await GetEstablishmentContactByIdAsync(establishmentContactId, httpContext);
        }
        else
        {
            return await GetEstablishmentContactsListAsync(
                new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize },
                httpContext, sortBy, sortOrder, establishmentId, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary);
        }
    }

    private async Task<IResult> GetEstablishmentContactByIdAsync(string establishmentContactId, HttpContext httpContext)
    {
        Result<EstablishmentContactId> contactIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentContactId>(establishmentContactId);
        if (!contactIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, contactIdResult.ErrorType, contactIdResult.Errors);
        }

        Result<EstablishmentContact> result = await _establishmentContactService
            .GetEstablishmentContactByIdAsync(contactIdResult.Value!);

        return result.IsSuccess && result.Value != null
            ? Results.Ok(_mapper.Map<EstablishmentContactResponse>(result.Value))
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", $"Establishment contact with ID '{establishmentContactId}' not found.")]);
    }

    private async Task<IResult> GetEstablishmentContactsListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? establishmentId,
        string? contactPersonFirstName,
        string? contactPersonMiddleName,
        string? contactPersonLastName,
        string? contactPhoneNumber,
        bool? isPrimary)
    {
        var spec = new EstablishmentContactSpecification(
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10,
            sortBy: sortBy,
            sortOrder: sortOrder,
            establishmentId: establishmentId,
            contactPersonFirstName: contactPersonFirstName,
            contactPersonMiddleName: contactPersonMiddleName,
            contactPersonLastName: contactPersonLastName,
            contactPhoneNumber: contactPhoneNumber,
            isPrimary: isPrimary
        );

        Result<int> countResult = await _establishmentContactService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<EstablishmentContact>> contactsResult = await _establishmentContactService.GetEstablishmentContactsAsync(spec);
        if (!contactsResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, contactsResult.ErrorType, contactsResult.Errors);
        }
        IEnumerable<EstablishmentContact> contacts = contactsResult.Value!;

        if (contacts == null || !contacts.Any())
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.NotFound, [new Error("NotFound", "No establishment contacts found matching the criteria.")]);
        }

        IEnumerable<EstablishmentContactResponse> responseData = _mapper.Map<IEnumerable<EstablishmentContactResponse>>(contacts);

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<EstablishmentContactResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, establishmentId, null, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary) : null
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
        string? establishmentContactId = null,
        string? contactPersonFirstName = null,
        string? contactPersonMiddleName = null,
        string? contactPersonLastName = null,
        string? contactPhoneNumber = null,
        bool? isPrimary = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(establishmentId))
        {
            link += $"&establishmentId={establishmentId}";
        }

        if (!string.IsNullOrEmpty(contactPersonFirstName))
        {
            link += $"&contactPersonFirstName={contactPersonFirstName}";
        }

        if (!string.IsNullOrEmpty(contactPersonMiddleName))
        {
            link += $"&contactPersonMiddleName={contactPersonMiddleName}";
        }

        if (!string.IsNullOrEmpty(contactPersonLastName))
        {
            link += $"&contactPersonLastName={contactPersonLastName}";
        }

        if (!string.IsNullOrEmpty(contactPhoneNumber))
        {
            link += $"&contactPhoneNumber={contactPhoneNumber}";
        }

        if (isPrimary.HasValue)
        {
            link += $"&isPrimary={isPrimary}";
        }

        return link;
    }
}
