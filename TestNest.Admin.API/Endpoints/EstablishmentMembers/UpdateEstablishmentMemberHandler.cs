using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public class UpdateEstablishmentMemberHandler(
    IEstablishmentMemberService establishmentMemberService,
    IMapper mapper)
{
    private readonly IEstablishmentMemberService _establishmentMemberService = establishmentMemberService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentMemberId,
        [FromBody] EstablishmentMemberForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentMemberId> memberIdResult = IdHelper.ValidateAndCreateId<EstablishmentMemberId>(establishmentMemberId);
        if (!memberIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, memberIdResult.ErrorType, memberIdResult.Errors);
        }

        Result<EstablishmentMember> updatedMember = await _establishmentMemberService.UpdateEstablishmentMemberAsync(memberIdResult.Value!, request);

        if (updatedMember.IsSuccess)
        {
            return Results.Ok(_mapper.Map<EstablishmentMemberResponse>(updatedMember.Value!));
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, updatedMember.ErrorType, updatedMember.Errors);
    }
}
