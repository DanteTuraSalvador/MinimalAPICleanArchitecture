using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentPhoneIdTests
{
    [Fact]
    public void EstablishmentPhoneId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentPhoneId = new EstablishmentPhoneId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentPhoneId.Value);
    }

    [Fact]
    public void EstablishmentPhoneId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentPhoneId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentPhoneId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentPhoneId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentPhoneId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentPhoneId = EstablishmentPhoneId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentPhoneId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentPhoneId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentPhoneId1 = EstablishmentPhoneId.Create(guid).Value;
        var establishmentPhoneId2 = EstablishmentPhoneId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentPhoneId1, establishmentPhoneId2);
        Assert.True(establishmentPhoneId1 == establishmentPhoneId2);
    }

    [Fact]
    public void EstablishmentPhoneId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentPhoneId1 = EstablishmentPhoneId.Create(guid1).Value;
        var establishmentPhoneId2 = EstablishmentPhoneId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentPhoneId1, establishmentPhoneId2);
        Assert.True(establishmentPhoneId1 != establishmentPhoneId2);
    }
}