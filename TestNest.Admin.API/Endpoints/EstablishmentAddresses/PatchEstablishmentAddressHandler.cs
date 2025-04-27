using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public class PatchEstablishmentAddressHandler(
    IEstablishmentAddressService establishmentAddressService)
{
    private readonly IEstablishmentAddressService _establishmentAddressService = establishmentAddressService;

    public async Task<IResult> HandleAsync(
        string establishmentAddressId,
        [FromBody] JsonPatchDocument<EstablishmentAddressPatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EstablishmentAddressId> addressIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentAddressId>(establishmentAddressId);
        if (!addressIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, addressIdResult.ErrorType, addressIdResult.Errors);
        }

        var addressPatchRequest = new EstablishmentAddressPatchRequest();
        patchDocument.ApplyTo(addressPatchRequest);

        var validationContext = new ValidationContext(addressPatchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(addressPatchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<EstablishmentAddressResponse> patchedAddress = await _establishmentAddressService
            .PatchEstablishmentAddressAsync(addressIdResult.Value!, addressPatchRequest);

        if (patchedAddress.IsSuccess)
        {
            return Results.Ok(patchedAddress.Value!);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, patchedAddress.ErrorType, patchedAddress.Errors);
    }
}
