using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.API.Endpoints.SocialMediaPlatforms;

public class DeleteSocialMediaPlatformHandler(
    ISocialMediaPlatformService socialMediaPlatformService,
    IErrorResponseService errorResponseService)
{
    private readonly ISocialMediaPlatformService _socialMediaPlatformService = socialMediaPlatformService;

    public async Task<IResult> HandleAsync(string socialMediaId, HttpContext httpContext)
    {
        Result<SocialMediaId> socialMediaIdResult = IdHelper.ValidateAndCreateId<SocialMediaId>(socialMediaId);
        if (!socialMediaIdResult.IsSuccess)
        {
            return MinimalApiErrorHelper.HandleErrorResponse(httpContext, socialMediaIdResult.ErrorType, socialMediaIdResult.Errors);
        }

        Result result = await _socialMediaPlatformService.DeleteSocialMediaPlatformAsync(socialMediaIdResult.Value!);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
