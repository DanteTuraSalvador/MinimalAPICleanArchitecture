using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.EstablishmentContacts;

public class DeleteEstablishmentContactHandler(
    IEstablishmentContactService establishmentContactService)
{
    private readonly IEstablishmentContactService _establishmentContactService = establishmentContactService;

    public async Task<IResult> HandleAsync(
        string establishmentContactId,
        HttpContext httpContext)
    {
        Result<EstablishmentContactId> contactIdResult = IdHelper
            .ValidateAndCreateId<EstablishmentContactId>(establishmentContactId);
        if (!contactIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, contactIdResult.ErrorType, contactIdResult.Errors);
        }

        Result result = await _establishmentContactService
            .DeleteEstablishmentContactAsync(contactIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
