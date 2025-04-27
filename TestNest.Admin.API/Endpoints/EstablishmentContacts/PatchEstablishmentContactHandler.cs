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

namespace TestNest.Admin.API.Endpoints.EstablishmentContacts;

public class PatchEstablishmentContactHandler(
    IEstablishmentContactService establishmentContactService)
{
    private readonly IEstablishmentContactService _establishmentContactService = establishmentContactService;

    public async Task<IResult> HandleAsync(
        string establishmentContactId,
        [FromBody] JsonPatchDocument<EstablishmentContactPatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EstablishmentContactId> contactIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentContactId>(establishmentContactId);
        if (!contactIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, contactIdResult.ErrorType, contactIdResult.Errors);
        }

        var contactPatchRequest = new EstablishmentContactPatchRequest();
        patchDocument.ApplyTo(contactPatchRequest);

        var validationContext = new ValidationContext(contactPatchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(contactPatchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<EstablishmentContactResponse> patchedContact = await _establishmentContactService
            .PatchEstablishmentContactAsync(contactIdResult.Value!, contactPatchRequest);

        if (patchedContact.IsSuccess)
        {
            return Results.Ok(patchedContact.Value!);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, patchedContact.ErrorType, patchedContact.Errors);
    }
}
