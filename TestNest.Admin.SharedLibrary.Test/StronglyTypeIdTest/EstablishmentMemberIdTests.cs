using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentMemberIdTests
{
    [Fact]
    public void EstablishmentMemberId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentMemberId = new EstablishmentMemberId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentMemberId.Value);
    }

    [Fact]
    public void EstablishmentMemberId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentMemberId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentMemberId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentMemberId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentMemberId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentMemberId = EstablishmentMemberId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentMemberId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentMemberId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentMemberId1 = EstablishmentMemberId.Create(guid).Value;
        var establishmentMemberId2 = EstablishmentMemberId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentMemberId1, establishmentMemberId2);
        Assert.True(establishmentMemberId1 == establishmentMemberId2);
    }

    [Fact]
    public void EstablishmentMemberId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentMemberId1 = EstablishmentMemberId.Create(guid1).Value;
        var establishmentMemberId2 = EstablishmentMemberId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentMemberId1, establishmentMemberId2);
        Assert.True(establishmentMemberId1 != establishmentMemberId2);
    }
}
