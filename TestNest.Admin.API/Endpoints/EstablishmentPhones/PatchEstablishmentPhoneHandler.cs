using MapsterMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public class PatchEstablishmentPhoneHandler(
    IEstablishmentPhoneService establishmentPhoneService,
    IMapper mapper)
{
    private readonly IEstablishmentPhoneService _establishmentPhoneService = establishmentPhoneService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentPhoneId,
        [FromBody] JsonPatchDocument<EstablishmentPhonePatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EstablishmentPhoneId> phoneIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentPhoneId>(establishmentPhoneId);
        if (!phoneIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, phoneIdResult.ErrorType, phoneIdResult.Errors);
        }

        var phonePatchRequest = new EstablishmentPhonePatchRequest();
        patchDocument.ApplyTo(phonePatchRequest);

        var validationContext = new ValidationContext(phonePatchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(phonePatchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<EstablishmentPhone> patchedPhone = await _establishmentPhoneService
            .PatchEstablishmentPhoneAsync(phoneIdResult.Value!, phonePatchRequest);

        if (patchedPhone.IsSuccess)
        {
            return Results.Ok(_mapper.Map<EstablishmentPhoneResponse>(patchedPhone.Value!));
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, patchedPhone.ErrorType, patchedPhone.Errors);
    }
}
