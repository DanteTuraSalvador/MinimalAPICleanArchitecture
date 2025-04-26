using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentAddressIdTests
{
    [Fact]
    public void EstablishmentAddressId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentAddressId = new EstablishmentAddressId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentAddressId.Value);
    }

    [Fact]
    public void EstablishmentAddressId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentAddressId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentAddressId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentAddressId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentAddressId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentAddressId = EstablishmentAddressId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentAddressId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentAddressId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentAddressId1 = EstablishmentAddressId.Create(guid).Value;
        var establishmentAddressId2 = EstablishmentAddressId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentAddressId1, establishmentAddressId2);
        Assert.True(establishmentAddressId1 == establishmentAddressId2);
    }

    [Fact]
    public void EstablishmentAddressId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentAddressId1 = EstablishmentAddressId.Create(guid1).Value;
        var establishmentAddressId2 = EstablishmentAddressId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentAddressId1, establishmentAddressId2);
        Assert.True(establishmentAddressId1 != establishmentAddressId2);
    }
}