using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.EstablishmentAddresses;

public static class EstablishmentAddressEndpoints
{
    public static void MapEstablishmentAddressApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/establishmentaddresses")
            .WithTags("EstablishmentAddresses");

        _ = group.MapPost("/", static async (
                [FromBody] EstablishmentAddressForCreationRequest request,
                HttpContext httpContext,
                CreateEstablishmentAddressHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEstablishmentAddress")
            .Produces<EstablishmentAddressResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new establishment address.")
            .WithDescription("Creates a new establishment address.");

        _ = group.MapPut("/{establishmentAddressId}", static async (
                string establishmentAddressId,
                [FromBody] EstablishmentAddressForUpdateRequest request,
                HttpContext httpContext,
                UpdateEstablishmentAddressHandler handler) =>
            await handler.HandleAsync(establishmentAddressId, request, httpContext))
            .WithName("UpdateEstablishmentAddress")
            .Produces<EstablishmentAddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing establishment address.")
            .WithDescription("Updates an existing establishment address.");

        _ = group.MapDelete("/{establishmentAddressId}", static async (
                string establishmentAddressId,
                HttpContext httpContext,
                DeleteEstablishmentAddressHandler handler) =>
            await handler.HandleAsync(establishmentAddressId, httpContext))
            .WithName("DeleteEstablishmentAddress")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an establishment address.")
            .WithDescription("Deletes an establishment address.");

        _ = group.MapGet("/", static async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEstablishmentAddressesHandler handler,
                string? sortBy = "Id",
                string? sortOrder = "asc",
                string? establishmentId = null,
                string? establishmentAddressId = null,
                string? city = null,
                string? municipality = null,
                string? province = null,
                string? region = null,
                bool? isPrimary = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, establishmentId, establishmentAddressId, city, municipality, province, region, isPrimary))
            .WithName("GetEstablishmentAddresses")
            .Produces<PaginatedResponse<EstablishmentAddressResponse>>(StatusCodes.Status200OK)
            .Produces<EstablishmentAddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of establishment addresses or a single address by ID.")
            .WithDescription("Retrieves a paginated list of establishment addresses with optional filtering, sorting, and pagination, or a single address by ID.");

        _ = group.MapPatch("/{establishmentAddressId}", static async (
                string establishmentAddressId,
                [FromBody] JsonPatchDocument<EstablishmentAddressPatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEstablishmentAddressHandler handler) =>
            await handler.HandleAsync(establishmentAddressId, patchDocument, httpContext))
            .WithName("PatchEstablishmentAddress")
            .Produces<EstablishmentAddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing establishment address.")
            .WithDescription("Partially updates an existing establishment address.");
    }
}
