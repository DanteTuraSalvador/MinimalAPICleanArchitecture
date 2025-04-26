using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public class CreateEstablishmentMemberHandler(
    IEstablishmentMemberService establishmentMemberService,
    IMapper mapper)
{
    private readonly IEstablishmentMemberService _establishmentMemberService = establishmentMemberService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] EstablishmentMemberForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentMember> result = await _establishmentMemberService.CreateEstablishmentMemberAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentMemberResponse dto = _mapper.Map<EstablishmentMemberResponse>(result.Value!);
            return Results.CreatedAtRoute("GetEstablishmentMembers", new { establishmentMemberId = dto.EstablishmentMemberId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
