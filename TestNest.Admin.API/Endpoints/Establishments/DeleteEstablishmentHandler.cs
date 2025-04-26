using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.Establishments;

public class DeleteEstablishmentHandler(
    IEstablishmentService establishmentService)
{
    private readonly IEstablishmentService _establishmentService = establishmentService;

    public async Task<IResult> HandleAsync(string establishmentId, HttpContext httpContext)
    {
        Result<EstablishmentId> establishmentIdResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
        if (!establishmentIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, establishmentIdResult.ErrorType, establishmentIdResult.Errors);
        }

        Result result = await _establishmentService.DeleteEstablishmentAsync(establishmentIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
