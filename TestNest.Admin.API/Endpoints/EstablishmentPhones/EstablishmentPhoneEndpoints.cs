using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.EstablishmentPhones;

public static class EstablishmentPhoneEndpoints
{
    public static void MapEstablishmentPhoneApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/establishmentphones")
            .WithTags("EstablishmentPhones");

        _ = group.MapPost("/", static async (
                [FromBody] EstablishmentPhoneForCreationRequest request,
                HttpContext httpContext,
                CreateEstablishmentPhoneHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEstablishmentPhone")
            .Produces<EstablishmentPhoneResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new phone number for an establishment.")
            .WithDescription("Creates a new phone number for an establishment.");

        _ = group.MapPut("/{establishmentPhoneId}", async (
                string establishmentPhoneId,
                [FromBody] EstablishmentPhoneForUpdateRequest request,
                HttpContext httpContext,
                UpdateEstablishmentPhoneHandler handler) =>
            await handler.HandleAsync(establishmentPhoneId, request, httpContext))
            .WithName("UpdateEstablishmentPhone")
            .Produces<EstablishmentPhoneResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing establishment phone number.")
            .WithDescription("Updates an existing establishment phone number.");

        _ = group.MapPatch("/{establishmentPhoneId}", async (
                string establishmentPhoneId,
                [FromBody] JsonPatchDocument<EstablishmentPhonePatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEstablishmentPhoneHandler handler) =>
            await handler.HandleAsync(establishmentPhoneId, patchDocument, httpContext))
            .WithName("PatchEstablishmentPhone")
            .Produces<EstablishmentPhoneResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing establishment phone number.")
            .WithDescription("Partially updates an existing establishment phone number.");

        _ = group.MapDelete("/{establishmentPhoneId}", async (
                string establishmentPhoneId,
                HttpContext httpContext,
                DeleteEstablishmentPhoneHandler handler) =>
            await handler.HandleAsync(establishmentPhoneId, httpContext))
            .WithName("DeleteEstablishmentPhone")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an establishment phone number.")
            .WithDescription("Deletes an establishment phone number.");

        _ = group.MapGet("/", async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEstablishmentPhonesHandler handler,
                string? sortBy = "Id",
                string? sortOrder = "asc",
                string? establishmentId = null,
                string? establishmentPhoneId = null,
                string? phoneNumber = null,
                bool? isPrimary = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, establishmentId, establishmentPhoneId, phoneNumber, isPrimary))
            .WithName("GetEstablishmentPhones")
            .Produces<PaginatedResponse<EstablishmentPhoneResponse>>(StatusCodes.Status200OK)
            .Produces<EstablishmentPhoneResponse>(StatusCodes.Status200OK) // For GetById
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of establishment phone numbers or a single phone number by ID.")
            .WithDescription("Retrieves a paginated list of establishment phone numbers with optional filtering, sorting, and pagination, or a single phone number by ID.");
    }
}
