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

namespace TestNest.Admin.API.Endpoints.Establishments;

public class PatchEstablishmentHandler(
    IEstablishmentService establishmentService,
    IMapper mapper)
{
    private readonly IEstablishmentService _establishmentService = establishmentService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentId,
        [FromBody] JsonPatchDocument<EstablishmentPatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        var establishmentPatchRequest = new EstablishmentPatchRequest();
        patchDocument.ApplyTo(establishmentPatchRequest);

        var validationContext = new ValidationContext(establishmentPatchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(establishmentPatchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<Establishment> result = await _establishmentService.PatchEstablishmentAsync(establishmentIdResult.Value!, establishmentPatchRequest);

        if (result.IsSuccess)
        {
            EstablishmentResponse dto = _mapper.Map<EstablishmentResponse>(result.Value!);
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
