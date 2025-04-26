using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public class CreateEstablishmentPhoneHandler(
    IEstablishmentPhoneService establishmentPhoneService,
    IMapper mapper)
{
    private readonly IEstablishmentPhoneService _establishmentPhoneService = establishmentPhoneService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] EstablishmentPhoneForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentPhone> result = await _establishmentPhoneService
            .CreateEstablishmentPhoneAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentPhoneResponse dto = _mapper.Map<EstablishmentPhoneResponse>(result.Value!);
            return Results.CreatedAtRoute("GetEstablishmentPhones", new { establishmentPhoneId = dto.EstablishmentPhoneId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
