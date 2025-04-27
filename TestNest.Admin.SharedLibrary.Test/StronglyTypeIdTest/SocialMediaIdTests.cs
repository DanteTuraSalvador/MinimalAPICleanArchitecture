using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class SocialMediaIdTests
{
    [Fact]
    public void SocialMediaId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var socialMediaId = new SocialMediaId();

        // Assert
        Assert.NotEqual(Guid.Empty, socialMediaId.Value);
    }

    [Fact]
    public void SocialMediaId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = SocialMediaId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void SocialMediaId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = SocialMediaId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void SocialMediaId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var socialMediaId = SocialMediaId.Create(validGuid).Value;

        // Act
        var stringRepresentation = socialMediaId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void SocialMediaId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var socialMediaId1 = SocialMediaId.Create(guid).Value;
        var socialMediaId2 = SocialMediaId.Create(guid).Value;

        // Assert
        Assert.Equal(socialMediaId1, socialMediaId2);
        Assert.True(socialMediaId1 == socialMediaId2);
    }

    [Fact]
    public void SocialMediaId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var socialMediaId1 = SocialMediaId.Create(guid1).Value;
        var socialMediaId2 = SocialMediaId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(socialMediaId1, socialMediaId2);
        Assert.True(socialMediaId1 != socialMediaId2);
    }
}
