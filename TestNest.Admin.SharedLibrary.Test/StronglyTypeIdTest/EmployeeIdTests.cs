using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EmployeeIdTests
{
    [Fact]
    public void EmployeeId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var employeeId = new EmployeeId();

        // Assert
        Assert.NotEqual(Guid.Empty, employeeId.Value);
    }

    [Fact]
    public void EmployeeId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EmployeeId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EmployeeId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EmployeeId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EmployeeId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var employeeId = EmployeeId.Create(validGuid).Value;

        // Act
        var stringRepresentation = employeeId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EmployeeId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var employeeId1 = EmployeeId.Create(guid).Value;
        var employeeId2 = EmployeeId.Create(guid).Value;

        // Assert
        Assert.Equal(employeeId1, employeeId2);
        Assert.True(employeeId1 == employeeId2);
    }

    [Fact]
    public void EmployeeId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var employeeId1 = EmployeeId.Create(guid1).Value;
        var employeeId2 = EmployeeId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(employeeId1, employeeId2);
        Assert.True(employeeId1 != employeeId2);
    }
}
