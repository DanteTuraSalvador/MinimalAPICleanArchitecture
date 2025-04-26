using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.API.Helpers;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.SocialMediaPlatform;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Endpoints.SocialMediaPlatforms;

public class CreateSocialMediaPlatformHandler(
    ISocialMediaPlatformService socialMediaPlatformService,
    IMapper mapper,
    IErrorResponseService errorResponseService)
{
    private readonly ISocialMediaPlatformService _socialMediaPlatformService = socialMediaPlatformService;
    private readonly IMapper _mapper = mapper;

    public async Task<IResult> HandleAsync(
        [FromBody] SocialMediaPlatformForCreationRequest request,
        HttpContext httpContext)
    {
        Result<SocialMediaPlatform> result = await _socialMediaPlatformService
            .CreateSocialMediaPlatformAsync(request);

        if (result.IsSuccess)
        {
            SocialMediaPlatformResponse dto = _mapper.Map<SocialMediaPlatformResponse>(result.Value!);
            return Results.CreatedAtRoute("GetSocialMediaPlatforms", new { socialMediaId = dto.Id }, dto);
        }

        return MinimalApiErrorHelper.HandleErrorResponse(httpContext, result.ErrorType, result.Errors);
    }
}
