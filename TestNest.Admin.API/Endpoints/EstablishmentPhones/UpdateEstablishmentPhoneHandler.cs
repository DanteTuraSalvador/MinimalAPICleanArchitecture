using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public class UpdateEstablishmentPhoneHandler(
    IEstablishmentPhoneService establishmentPhoneService,
    IMapper mapper)
{
    private readonly IEstablishmentPhoneService _establishmentPhoneService = establishmentPhoneService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        string establishmentPhoneId,
        [FromBody] EstablishmentPhoneForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentPhoneId> phoneIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentPhoneId>(establishmentPhoneId);
        if (!phoneIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, phoneIdResult.ErrorType, phoneIdResult.Errors);
        }

        Result<EstablishmentId> establishmentIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentId>(request.EstablishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result<EstablishmentPhone> updatedPhone = await _establishmentPhoneService
            .UpdateEstablishmentPhoneAsync(phoneIdResult.Value!, request);

        if (updatedPhone.IsSuccess)
        {
            return Results.Ok(_mapper.Map<EstablishmentPhoneResponse>(updatedPhone.Value!));
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, updatedPhone.ErrorType, updatedPhone.Errors);
    }
}
