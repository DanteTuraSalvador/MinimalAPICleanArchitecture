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

public class CreateEstablishmentContactHandler(
    IEstablishmentContactService establishmentContactService,
    IMapper mapper)
{
    private readonly IEstablishmentContactService _establishmentContactService = establishmentContactService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] EstablishmentContactForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentContact> result = await _establishmentContactService
            .CreateEstablishmentContactAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentContactResponse dto = _mapper.Map<EstablishmentContactResponse>(result.Value!);
            return Results.CreatedAtRoute("GetEstablishmentContacts", new { establishmentContactId = dto.EstablishmentContactId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
