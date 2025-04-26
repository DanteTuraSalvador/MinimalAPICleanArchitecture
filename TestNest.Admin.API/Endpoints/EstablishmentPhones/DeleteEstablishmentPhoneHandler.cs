using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public class DeleteEstablishmentPhoneHandler(
    IEstablishmentPhoneService establishmentPhoneService)
{
    private readonly IEstablishmentPhoneService _establishmentPhoneService = establishmentPhoneService;

    public async Task<IResult> HandleAsync(
        string establishmentPhoneId,
        HttpContext httpContext)
    {
        Result<EstablishmentPhoneId> phoneIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentPhoneId>(establishmentPhoneId);
        if (!phoneIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, phoneIdResult.ErrorType, phoneIdResult.Errors);
        }

        Result result = await _establishmentPhoneService
            .DeleteEstablishmentPhoneAsync(phoneIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
