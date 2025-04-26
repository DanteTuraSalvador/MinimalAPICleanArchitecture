using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentSocialMediaIdTests
{
    [Fact]
    public void EstablishmentSocialMediaId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentSocialMediaId = new EstablishmentSocialMediaId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentSocialMediaId.Value);
    }

    [Fact]
    public void EstablishmentSocialMediaId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentSocialMediaId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentSocialMediaId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentSocialMediaId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentSocialMediaId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentSocialMediaId = EstablishmentSocialMediaId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentSocialMediaId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentSocialMediaId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentSocialMediaId1 = EstablishmentSocialMediaId.Create(guid).Value;
        var establishmentSocialMediaId2 = EstablishmentSocialMediaId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentSocialMediaId1, establishmentSocialMediaId2);
        Assert.True(establishmentSocialMediaId1 == establishmentSocialMediaId2);
    }

    [Fact]
    public void EstablishmentSocialMediaId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentSocialMediaId1 = EstablishmentSocialMediaId.Create(guid1).Value;
        var establishmentSocialMediaId2 = EstablishmentSocialMediaId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentSocialMediaId1, establishmentSocialMediaId2);
        Assert.True(establishmentSocialMediaId1 != establishmentSocialMediaId2);
    }
}