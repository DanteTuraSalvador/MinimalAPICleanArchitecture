using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Establishments;

public class UpdateEstablishmentHandler(
    IEstablishmentService establishmentService,
    IMapper mapper)
{
    private readonly IEstablishmentService _establishmentService = establishmentService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentId,
        [FromBody] EstablishmentForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<Establishment> result = await _establishmentService.UpdateEstablishmentAsync(establishmentIdResult.Value!, request);

        if (result.IsSuccess)
        {
            EstablishmentResponse dto = _mapper.Map<EstablishmentResponse>(result.Value!);
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
