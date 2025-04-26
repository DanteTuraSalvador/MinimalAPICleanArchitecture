using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.Establishments;

public static class EstablishmentEndpoints
{
    public static void MapEstablishmentApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/establishments")
            .WithTags("Establishments");

        _ = group.MapPost("/", static async (
                [FromBody] EstablishmentForCreationRequest request,
                HttpContext httpContext,
                CreateEstablishmentHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEstablishment")
            .Produces<EstablishmentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new establishment.")
            .WithDescription("Creates a new establishment.");

        _ = group.MapPut("/{establishmentId}", static async (
                string establishmentId,
                [FromBody] EstablishmentForUpdateRequest request,
                HttpContext httpContext,
                UpdateEstablishmentHandler handler) =>
            await handler.HandleAsync(establishmentId, request, httpContext))
            .WithName("UpdateEstablishment")
            .Produces<EstablishmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing establishment.")
            .WithDescription("Updates an existing establishment.");

        _ = group.MapPatch("/{establishmentId}", static async (
                string establishmentId,
                [FromBody] JsonPatchDocument<EstablishmentPatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEstablishmentHandler handler) =>
            await handler.HandleAsync(establishmentId, patchDocument, httpContext))
            .WithName("PatchEstablishment")
            .Produces<EstablishmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing establishment.")
            .WithDescription("Partially updates an existing establishment.");

        _ = group.MapDelete("/{establishmentId}", static async (
                string establishmentId,
                HttpContext httpContext,
                DeleteEstablishmentHandler handler) =>
            await handler.HandleAsync(establishmentId, httpContext))
            .WithName("DeleteEstablishment")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an establishment.")
            .WithDescription("Deletes an establishment.");

        _ = group.MapGet("/", static async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEstablishmentsHandler handler,
                string? sortBy = "EstablishmentId",
                string? sortOrder = "asc",
                string? establishmentId = null,
                string? establishmentName = null,
                string? establishmentEmail = null,
                int? establishmentStatusId = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, establishmentId, establishmentName, establishmentEmail, establishmentStatusId))
            .WithName("GetEstablishments")
            .Produces<PaginatedResponse<EstablishmentResponse>>(StatusCodes.Status200OK)
            .Produces<EstablishmentResponse>(StatusCodes.Status200OK) // For GetById
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of establishments or a single establishment by ID.")
            .WithDescription("Retrieves a paginated list of establishments with optional filtering, sorting, and pagination, or a single establishment by ID.");
    }
}
