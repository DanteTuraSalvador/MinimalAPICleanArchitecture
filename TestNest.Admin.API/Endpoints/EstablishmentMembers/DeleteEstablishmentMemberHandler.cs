using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public class DeleteEstablishmentMemberHandler(
    IEstablishmentMemberService establishmentMemberService)
{
    private readonly IEstablishmentMemberService _establishmentMemberService = establishmentMemberService;

    public async Task<IResult> HandleAsync(
        string establishmentMemberId,
        HttpContext httpContext)
    {
        Result<EstablishmentMemberId> memberIdResult = IdHelper.ValidateAndCreateId<EstablishmentMemberId>(establishmentMemberId);
        if (!memberIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, memberIdResult.ErrorType, memberIdResult.Errors);
        }

        Result result = await _establishmentMemberService.DeleteEstablishmentMemberAsync(memberIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
