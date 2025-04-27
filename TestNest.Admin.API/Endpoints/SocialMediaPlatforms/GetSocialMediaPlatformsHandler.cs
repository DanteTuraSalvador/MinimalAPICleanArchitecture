using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Specifications.SoicalMediaPlatfomrSpecifications;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Paginations;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.SocialMediaPlatforms;

public class GetSocialMediaPlatformsHandler(
    ISocialMediaPlatformService socialMediaPlatformService,
    IErrorResponseService errorResponseService)
{
    private readonly ISocialMediaPlatformService _socialMediaPlatformService = socialMediaPlatformService;

    public async Task<IResult> HandleAsync(
        [AsParameters] PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy = "Id",
        string? sortOrder = "asc",
        string? name = null,
        string? platformURL = null,
        string? socialMediaId = null)
    {
        int pageNumber = paginationRequest.PageNumber ?? 1;
        int pageSize = paginationRequest.PageSize ?? 10;

        if (!string.IsNullOrEmpty(socialMediaId))
        {
            return await GetSocialMediaPlatformByIdAsync(socialMediaId, httpContext);
        }
        else
        {
            return await GetSocialMediaPlatformsListAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize }, httpContext, sortBy, sortOrder, name, platformURL);
        }
    }

    private async Task<IResult> GetSocialMediaPlatformByIdAsync(string socialMediaId, HttpContext httpContext)
    {
        Result<SocialMediaId> socialMediaIdResult = IdHelper.ValidateAndCreateId<SocialMediaId>(socialMediaId);
        if (!socialMediaIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, socialMediaIdResult.ErrorType, socialMediaIdResult.Errors);
        }

        Result<SocialMediaPlatformResponse> result = await _socialMediaPlatformService
            .GetSocialMediaPlatformByIdAsync(socialMediaIdResult.Value!);

        return result.IsSuccess
            ? Results.Ok(result.Value!)
            : MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }

    private async Task<IResult> GetSocialMediaPlatformsListAsync(
        PaginationRequest paginationRequest,
        HttpContext httpContext,
        string? sortBy,
        string? sortOrder,
        string? name,
        string? platformURL)
    {
        var spec = new SocialMediaPlatformSpecification(
            name: name,
            platformURL: platformURL,
            sortBy: sortBy,
            sortDirection: sortOrder,
            pageNumber: paginationRequest.PageNumber ?? 1,
            pageSize: paginationRequest.PageSize ?? 10
        );

        Result<int> countResult = await _socialMediaPlatformService.CountAsync(spec);
        if (!countResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, countResult.ErrorType, countResult.Errors);
        }
        int totalCount = countResult.Value;

        Result<IEnumerable<SocialMediaPlatformResponse>> socialMediaPlatformsResult = await _socialMediaPlatformService.GetAllSocialMediaPlatformsAsync(spec);
        if (!socialMediaPlatformsResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, socialMediaPlatformsResult.ErrorType, socialMediaPlatformsResult.Errors);
        }

        IEnumerable<SocialMediaPlatformResponse> socialMediaPlatforms = socialMediaPlatformsResult.Value!;

        if (socialMediaPlatforms == null || !socialMediaPlatforms.Any())
        {
            return Results.NotFound();
        }

        IEnumerable<SocialMediaPlatformResponse> responseData = socialMediaPlatforms!;

        int totalPages = (int)Math.Ceiling((double)totalCount / (paginationRequest.PageSize ?? 10));

        var paginatedResponse = new PaginatedResponse<SocialMediaPlatformResponse>
        {
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber ?? 1,
            PageSize = paginationRequest.PageSize ?? 10,
            TotalPages = totalPages,
            Data = responseData,
            Links = new PaginatedLinks
            {
                First = GeneratePaginationLink(httpContext, 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, name, platformURL),
                Last = totalPages > 0 ? GeneratePaginationLink(httpContext, totalPages, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, name, platformURL) : null,
                Next = paginationRequest.PageNumber < totalPages ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) + 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, name, platformURL) : null,
                Previous = paginationRequest.PageNumber > 1 ? GeneratePaginationLink(httpContext, (paginationRequest.PageNumber ?? 1) - 1, paginationRequest.PageSize ?? 10, sortBy!, sortOrder!, name, platformURL) : null
            }
        };

        return Results.Ok(paginatedResponse);
    }

    private static string GeneratePaginationLink(
        HttpContext httpContext,
        int pageNumber,
        int pageSize,
        string sortBy,
        string sortOrder,
        string? name = null,
        string? platformURL = null)
    {
        string link = $"{httpContext.Request.PathBase}{httpContext.Request.Path}?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";
        if (!string.IsNullOrEmpty(name))
            link += $"&name={name}";
        if (!string.IsNullOrEmpty(platformURL))
            link += $"&platformURL={platformURL}";
        return link;
    }
}
