using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.SocialMedias;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface ISocialMediaPlatformRepository
//{
//    Task<Result<SocialMediaPlatform>> GetSocialMediaPlatformByNameAsync(string socialMediaAccountName);

//    Task<Result<SocialMediaPlatform>> GetByIdAsync(SocialMediaId socialMediaId);

//    Task<Result<IEnumerable<SocialMediaPlatform>>> GetAllAsync();

//    Task<Result<IEnumerable<SocialMediaPlatform>>> ListAsync(ISpecification<SocialMediaPlatform> spec);

//    Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec);

//    Task<Result> AddAsync(SocialMediaPlatform socialMediaPlatform);

//    Task<Result> UpdateAsync(SocialMediaPlatform socialMediaPlatform);

//    Task<Result> DeleteAsync(SocialMediaId socialMediaId);

//    Task DetachAsync(SocialMediaPlatform platform);
//}

public interface ISocialMediaPlatformRepository : IGenericRepository<SocialMediaPlatform, SocialMediaId>
{
    Task<Result<SocialMediaPlatform>> GetSocialMediaPlatformByNameAsync(string socialMediaAccountName);

    Task DetachAsync(SocialMediaPlatform platform);

    Task<Result<IEnumerable<SocialMediaPlatform>>> ListAsync(ISpecification<SocialMediaPlatform> spec);

    Task<Result<int>> CountAsync(ISpecification<SocialMediaPlatform> spec);
}
