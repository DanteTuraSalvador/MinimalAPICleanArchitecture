using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.SocialMedias;

public sealed class SocialMediaPlatform : BaseEntity<SocialMediaId>
{
    public SocialMediaId SocialMediaId => Id;
    public SocialMediaName SocialMediaName { get; private set; }

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<SocialMediaPlatform> _empty = new(() => new SocialMediaPlatform());

    public static SocialMediaPlatform Empty() => _empty.Value;

    private SocialMediaPlatform() : base(SocialMediaId.Empty()) => SocialMediaName = SocialMediaName.Empty();

    private SocialMediaPlatform(SocialMediaId id, SocialMediaName socialMediaName) : base(id) => SocialMediaName = socialMediaName;

    public static Result<SocialMediaPlatform> Create(SocialMediaName socialMediaName)
    {
        var socialMediaNameResult = SocialMediaName.Create(socialMediaName.Name, socialMediaName.PlatformURL);

        return socialMediaNameResult.IsSuccess
            ? Result<SocialMediaPlatform>.Success(new SocialMediaPlatform(SocialMediaId.New(), socialMediaNameResult.Value!))
            : Result<SocialMediaPlatform>.Failure(ErrorType.Validation, socialMediaNameResult.Errors);
    }

    public Result<SocialMediaPlatform> WithSocialMediaName(SocialMediaName newName)
    {
        var socialMediaNameResult = SocialMediaName.Create(newName.Name, newName.PlatformURL);

        return socialMediaNameResult.IsSuccess
            ? Result<SocialMediaPlatform>.Success(new SocialMediaPlatform(Id, socialMediaNameResult.Value!))
            : Result<SocialMediaPlatform>.Failure(ErrorType.Validation, socialMediaNameResult.Errors);
    }

    public override string ToString() => $"{SocialMediaName.Name} ({SocialMediaName.PlatformURL})";
}