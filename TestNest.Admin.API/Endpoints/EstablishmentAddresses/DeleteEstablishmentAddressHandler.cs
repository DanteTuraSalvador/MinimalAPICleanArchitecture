using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public class DeleteEstablishmentAddressHandler(
    IEstablishmentAddressService establishmentAddressService)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;

    public async Task<IResult> HandleAsync(
        string establishmentAddressId,
        HttpContext httpContext)
    {
        Result<EstablishmentAddressId> addressIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentAddressId>(establishmentAddressId);
        if (!addressIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, addressIdResult.ErrorType, addressIdResult.Errors);
        }

        Result result = await _establishmentAddressService
            .DeleteEstablishmentAddressAsync(addressIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
