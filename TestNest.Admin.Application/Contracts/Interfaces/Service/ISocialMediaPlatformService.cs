using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.SocialMediaPlatform;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface ISocialMediaPlatformService
{
    Task<Result<SocialMediaPlatformResponse>> CreateSocialMediaPlatformAsync(
        SocialMediaPlatformForCreationRequest socialMediaPlatformForCreationRequest);

    Task<Result<SocialMediaPlatformResponse>> UpdateSocialMediaPlatformAsync(
        SocialMediaId socialMediaId,
        SocialMediaPlatformForUpdateRequest socialMediaPlatformUpdateDto);

    Task<Result> DeleteSocialMediaPlatformAsync(SocialMediaId socialMediaId);

    Task<Result<SocialMediaPlatformResponse>> GetSocialMediaPlatformByIdAsync(SocialMediaId socialMediaId);

    Task<Result<IEnumerable<SocialMediaPlatformResponse>>> GetAllSocialMediaPlatformsAsync(ISpecification<SocialMediaPlatform> spec); // Updated return type

    Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec);
}
