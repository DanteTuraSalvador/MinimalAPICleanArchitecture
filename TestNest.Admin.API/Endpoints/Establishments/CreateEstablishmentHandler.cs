using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.Establishments;

public class CreateEstablishmentHandler(
    IEstablishmentService establishmentService)
{
    private readonly IEstablishmentService _establishmentService = establishmentService;

    public async Task<IResult> HandleAsync(
        [FromBody] EstablishmentForCreationRequest request,
        HttpContext httpContext)
    {
        Result<EstablishmentResponse> result = await _establishmentService.CreateEstablishmentAsync(request);

        if (result.IsSuccess)
        {
            EstablishmentResponse dto = result.Value!;
            return Results.CreatedAtRoute("GetEstablishments", new { establishmentId = dto.EstablishmentId }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
