using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;
public class CreateEstablishmentAddressHandler(
    IEstablishmentAddressService establishmentAddressService,
    IMapper mapper)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] EstablishmentAddressForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentAddress> result = await _establishmentAddressService
            .CreateEstablishmentAddressAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentAddressResponse dto = _mapper.Map<EstablishmentAddressResponse>(result.Value!);
            return Results.CreatedAtRoute("GetEstablishmentAddresses", new { establishmentAddressId = dto.EstablishmentAddressId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
