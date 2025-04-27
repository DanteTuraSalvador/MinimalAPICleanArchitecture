using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.SocialMediaPlatform;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.SocialMediaPlatforms;

public class UpdateSocialMediaPlatformHandler(
    ISocialMediaPlatformService socialMediaPlatformService,
    IErrorResponseService errorResponseService)
{
    private readonly ISocialMediaPlatformService _socialMediaPlatformService = socialMediaPlatformService;

    public async Task<IResult> HandleAsync(
        string socialMediaId,
        [FromBody] SocialMediaPlatformForUpdateRequest request,
        HttpContext httpContext)
    {
        Result<SocialMediaId> socialMediaIdResult = IdHelper.ValidateAndCreateId<SocialMediaId>(socialMediaId);
        if (!socialMediaIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, socialMediaIdResult.ErrorType, socialMediaIdResult.Errors);
        }

        Result<SocialMediaPlatformResponse> result = await _socialMediaPlatformService
            .UpdateSocialMediaPlatformAsync(socialMediaIdResult.Value!, request);

        if (result.IsSuccess)
        {
            SocialMediaPlatformResponse dto = result.Value!;
            return Results.Ok(dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
