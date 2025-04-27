using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public class CreateEstablishmentAddressHandler(
    IEstablishmentAddressService establishmentAddressService)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;

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

        Result<EstablishmentAddressResponse> result = await _establishmentAddressService
            .CreateEstablishmentAddressAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentAddressResponse dto = result.Value!;
            return Results.CreatedAtRoute("GetEstablishmentAddresses", new { establishmentAddressId = dto.EstablishmentAddressId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
