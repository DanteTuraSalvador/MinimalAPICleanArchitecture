using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentContacts;

public class UpdateEstablishmentContactHandler(
    IEstablishmentContactService establishmentContactService,
    IMapper mapper)
{
    private readonly IEstablishmentContactService _establishmentContactService = establishmentContactService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentContactId,
        [FromBody] EstablishmentContactForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentContactId> contactIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentContactId>(establishmentContactId);
        if (!contactIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, contactIdResult.ErrorType, contactIdResult.Errors);
        }

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentContact> updatedContact = await _establishmentContactService
            .UpdateEstablishmentContactAsync(contactIdResult.Value!, request);

        if (updatedContact.IsSuccess)
        {
            return Results.Ok(_mapper.Map<EstablishmentContactResponse>(updatedContact.Value!));
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, updatedContact.ErrorType, updatedContact.Errors);
    }
}
