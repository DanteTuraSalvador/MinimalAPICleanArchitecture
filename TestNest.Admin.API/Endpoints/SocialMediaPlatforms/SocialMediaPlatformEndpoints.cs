using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.SocialMediaPlatform;

namespace TestNest.Admin.API.Endpoints.SocialMediaPlatforms;

public static class SocialMediaPlatformEndpoints
{
    public static void MapSocialMediaPlatformApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/socialmediaplatforms")
            .WithTags("SocialMediaPlatforms");

        _ = group.MapPost("/", async (
                [FromBody] SocialMediaPlatformForCreationRequest request,
                HttpContext httpContext,
                CreateSocialMediaPlatformHandler handler) =>
            await handler.HandleAsync(request, httpContext))
            .WithName("CreateSocialMediaPlatform")
            .Produces<SocialMediaPlatformResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates a new social media platform.")
            .WithDescription("Creates a new social media platform.");

        _ = group.MapPut("/{socialMediaId}", async (
                string socialMediaId,
                [FromBody] SocialMediaPlatformForUpdateRequest request,
                HttpContext httpContext,
                UpdateSocialMediaPlatformHandler handler) =>
            await handler.HandleAsync(socialMediaId, request, httpContext))
            .WithName("UpdateSocialMediaPlatform")
            .Produces<SocialMediaPlatformResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing social media platform.")
            .WithDescription("Updates an existing social media platform.");

        _ = group.MapDelete("/{socialMediaId}", async (
                string socialMediaId,
                HttpContext httpContext,
                DeleteSocialMediaPlatformHandler handler) =>
            await handler.HandleAsync(socialMediaId, httpContext))
            .WithName("DeleteSocialMediaPlatform")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes a social media platform.")
            .WithDescription("Deletes a social media platform.");

        _ = group.MapGet("/", async (
                [AsParameters] PaginationRequest paginationRequest,
                HttpContext httpContext,
                GetSocialMediaPlatformsHandler handler,
                string? sortBy = "Id",
                string? sortOrder = "asc",
                string? name = null,
                string? platformURL = null,
                string? socialMediaId = null) =>
            await handler.HandleAsync(paginationRequest, httpContext, sortBy, sortOrder, name, platformURL, socialMediaId))
            .WithName("GetSocialMediaPlatforms")
            .Produces<PaginatedResponse<SocialMediaPlatformResponse>>(StatusCodes.Status200OK)
            .Produces<SocialMediaPlatformResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Retrieves a list of social media platforms or a single platform by ID.")
            .WithDescription("Retrieves a paginated list of social media platforms with optional filtering, sorting, and pagination, or a single social media platform by ID.");
    }
}
