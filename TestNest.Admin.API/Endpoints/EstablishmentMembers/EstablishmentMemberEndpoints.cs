using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;

namespace TestNest.Admin.API.Endpoints.EstablishmentMembers;

public static class EstablishmentMemberEndpoints
{
    public static void MapEstablishmentMemberApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/establishmentmembers")
            .WithTags("EstablishmentMembers");

        _ = group.MapPost("/", static async (
                [FromBody] EstablishmentMemberForCreationRequest request,
                HttpContext httpContext,
                CreateEstablishmentMemberHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateEstablishmentMember")
            .Produces<EstablishmentMemberResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new establishment member.")
            .WithDescription("Creates a new establishment member.");

        _ = group.MapPut("/{establishmentMemberId}", async (
                string establishmentMemberId,
                [FromBody] EstablishmentMemberForUpdateRequest request,
                HttpContext httpContext,
                UpdateEstablishmentMemberHandler handler) =>
            await handler.HandleAsync(establishmentMemberId, request, httpContext))
            .WithName("UpdateEstablishmentMember")
            .Produces<EstablishmentMemberResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing establishment member.")
            .WithDescription("Updates an existing establishment member.");

        _ = group.MapPatch("/{establishmentMemberId}", async (
                string establishmentMemberId,
                [FromBody] JsonPatchDocument<EstablishmentMemberPatchRequest> patchDocument,
                HttpContext httpContext,
                PatchEstablishmentMemberHandler handler) =>
            await handler.HandleAsync(establishmentMemberId, patchDocument, httpContext))
            .WithName("PatchEstablishmentMember")
            .Produces<EstablishmentMemberResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Partially updates an existing establishment member.")
            .WithDescription("Partially updates an existing establishment member.");

        _ = group.MapDelete("/{establishmentMemberId}", async (
                string establishmentMemberId,
                HttpContext httpContext,
                DeleteEstablishmentMemberHandler handler) =>
            await handler.HandleAsync(establishmentMemberId, httpContext))
            .WithName("DeleteEstablishmentMember")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes an establishment member.")
            .WithDescription("Deletes an establishment member.");

        _ = group.MapGet("/", async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetEstablishmentMembersHandler handler,
                string? sortBy = "Id",
                string? sortOrder = "asc",
                string? establishmentMemberId = null,
                string? establishmentId = null,
                string? employeeId = null,
                string? memberTitle = null,
                string? memberDescription = null,
                string? memberTag = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, establishmentMemberId, establishmentId, employeeId, memberTitle, memberDescription, memberTag))
            .WithName("GetEstablishmentMembers")
            .Produces<PaginatedResponse<EstablishmentMemberResponse>>(StatusCodes.Status200OK)
            .Produces<EstablishmentMemberResponse>(StatusCodes.Status200OK) 
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of establishment members or a single member by ID.")
            .WithDescription("Retrieves a paginated list of establishment members with optional filtering, sorting, and pagination, or a single member by ID.");
    }
}
