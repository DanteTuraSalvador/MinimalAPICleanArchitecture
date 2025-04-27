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

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public class PatchEstablishmentMemberHandler(
    IEstablishmentMemberService establishmentMemberService)
{
    private readonly IEstablishmentMemberService _establishmentMemberService = establishmentMemberService;

    public async Task<IResult> HandleAsync(
        string establishmentMemberId,
        [FromBody] JsonPatchDocument<EstablishmentMemberPatchRequest> patchDocument,
        HttpContext httpContext)
    {
        Result<EstablishmentMemberId> memberIdResult = IdHelper.ValidateAndCreateId<EstablishmentMemberId>(establishmentMemberId);
        if (!memberIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, memberIdResult.ErrorType, memberIdResult.Errors);
        }

        var patchRequest = new EstablishmentMemberPatchRequest();
        patchDocument.ApplyTo(patchRequest);

        var validationContext = new ValidationContext(patchRequest, httpContext.RequestServices, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(patchRequest, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => new Error("ValidationError", vr.ErrorMessage)).ToList();
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, ErrorType.Validation, errors);
        }

        Result<EstablishmentMemberResponse> patchedMember = await _establishmentMemberService.PatchEstablishmentMemberAsync(memberIdResult.Value!, patchRequest);

        if (patchedMember.IsSuccess)
        {
            return Results.Ok(patchedMember.Value!);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, patchedMember.ErrorType, patchedMember.Errors);
    }
}
