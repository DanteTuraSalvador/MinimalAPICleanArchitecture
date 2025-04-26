using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentIdTests
{
    [Fact]
    public void EstablishmentId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentId = new EstablishmentId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentId.Value);
    }

    [Fact]
    public void EstablishmentId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentId = EstablishmentId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentId1 = EstablishmentId.Create(guid).Value;
        var establishmentId2 = EstablishmentId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentId1, establishmentId2);
        Assert.True(establishmentId1 == establishmentId2);
    }

    [Fact]
    public void EstablishmentId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentId1 = EstablishmentId.Create(guid1).Value;
        var establishmentId2 = EstablishmentId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentId1, establishmentId2);
        Assert.True(establishmentId1 != establishmentId2);
    }
}