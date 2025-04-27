using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public class UpdateEstablishmentAddressHandler(
    IEstablishmentAddressService establishmentAddressService)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;

    public async Task<IResult> HandleAsync(
        string establishmentAddressId,
        [FromBody] EstablishmentAddressForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentAddressId> addressIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentAddressId>(establishmentAddressId);
        if (!addressIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, addressIdResult.ErrorType, addressIdResult.Errors);
        }

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentAddressResponse> updatedAddress = await _establishmentAddressService
            .UpdateEstablishmentAddressAsync(addressIdResult.Value!, request);

        if (updatedAddress.IsSuccess)
        {
            return Results.Ok(updatedAddress.Value!);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, updatedAddress.ErrorType, updatedAddress.Errors);
    }
}
