using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EstablishmentContactIdTests
{
    [Fact]
    public void EstablishmentContactId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var establishmentContactId = new EstablishmentContactId();

        // Assert
        Assert.NotEqual(Guid.Empty, establishmentContactId.Value);
    }

    [Fact]
    public void EstablishmentContactId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EstablishmentContactId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EstablishmentContactId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EstablishmentContactId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EstablishmentContactId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var establishmentContactId = EstablishmentContactId.Create(validGuid).Value;

        // Act
        var stringRepresentation = establishmentContactId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EstablishmentContactId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var establishmentContactId1 = EstablishmentContactId.Create(guid).Value;
        var establishmentContactId2 = EstablishmentContactId.Create(guid).Value;

        // Assert
        Assert.Equal(establishmentContactId1, establishmentContactId2);
        Assert.True(establishmentContactId1 == establishmentContactId2);
    }

    [Fact]
    public void EstablishmentContactId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var establishmentContactId1 = EstablishmentContactId.Create(guid1).Value;
        var establishmentContactId2 = EstablishmentContactId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(establishmentContactId1, establishmentContactId2);
        Assert.True(establishmentContactId1 != establishmentContactId2);
    }
}