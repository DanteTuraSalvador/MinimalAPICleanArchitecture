using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.EstablishmentContacts;

public static class EstablishmentContactEndpoints
{
    public static void MapEstablishmentContactApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/establishmentcontacts")
            .WithTags("EstablishmentContacts");

        _ = group.MapPost("/", static async (
                [FromBody] EstablishmentContactForCreationRequest request,
                HttpContext httpContext,
                CreateEstablishmentContactHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEstablishmentContact")
            .Produces<EstablishmentContactResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new establishment contact.")
            .WithDescription("Creates a new establishment contact.");

        _ = group.MapPut("/{establishmentContactId}", async (
                string establishmentContactId,
                [FromBody] EstablishmentContactForUpdateRequest request,
                HttpContext httpContext,
                UpdateEstablishmentContactHandler handler) =>
            await handler.HandleAsync(establishmentContactId, request, httpContext))
            .WithName("UpdateEstablishmentContact")
            .Produces<EstablishmentContactResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing establishment contact.")
            .WithDescription("Updates an existing establishment contact.");

        _ = group.MapPatch("/{establishmentContactId}", async (
                string establishmentContactId,
                [FromBody] JsonPatchDocument<EstablishmentContactPatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEstablishmentContactHandler handler) =>
            await handler.HandleAsync(establishmentContactId, patchDocument, httpContext))
            .WithName("PatchEstablishmentContact")
            .Produces<EstablishmentContactResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing establishment contact.")
            .WithDescription("Partially updates an existing establishment contact.");

        _ = group.MapDelete("/{establishmentContactId}", async (
                string establishmentContactId,
                HttpContext httpContext,
                DeleteEstablishmentContactHandler handler) =>
            await handler.HandleAsync(establishmentContactId, httpContext))
            .WithName("DeleteEstablishmentContact")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an establishment contact.")
            .WithDescription("Deletes an establishment contact.");

        _ = group.MapGet("/", async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEstablishmentContactsHandler handler,
                string? sortBy = "Id",
                string? sortOrder = "asc",
                string? establishmentId = null,
                string? establishmentContactId = null,
                string? contactPersonFirstName = null,
                string? contactPersonMiddleName = null,
                string? contactPersonLastName = null,
                string? contactPhoneNumber = null,
                bool? isPrimary = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, establishmentId, establishmentContactId, contactPersonFirstName, contactPersonMiddleName, contactPersonLastName, contactPhoneNumber, isPrimary))
            .WithName("GetEstablishmentContacts")
            .Produces<PaginatedResponse<EstablishmentContactResponse>>(StatusCodes.Status200OK)
            .Produces<EstablishmentContactResponse>(StatusCodes.Status200OK) // For GetById
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of establishment contacts or a single contact by ID.")
            .WithDescription("Retrieves a paginated list of establishment contacts with optional filtering, sorting, and pagination, or a single contact by ID.");
    }
}
