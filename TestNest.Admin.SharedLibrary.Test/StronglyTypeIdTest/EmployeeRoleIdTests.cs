using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.SharedLibrary.Test.StronglyTypeIdTest;

public class EmployeeRoleIdTests
{
    [Fact]
    public void EmployeeRoleId_Constructor_SetsNewGuid()
    {
        // Arrange & Act
        var employeeRoleId = new EmployeeRoleId();

        // Assert
        Assert.NotEqual(Guid.Empty, employeeRoleId.Value);
    }

    [Fact]
    public void EmployeeRoleId_Create_WithValidGuid_ReturnsSuccessResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act
        var result = EmployeeRoleId.Create(validGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validGuid, result.Value.Value);
    }

    [Fact]
    public void EmployeeRoleId_Create_WithEmptyGuid_ReturnsFailureResult()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = EmployeeRoleId.Create(emptyGuid);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void EmployeeRoleId_ToString_ReturnsGuidAsString()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var employeeRoleId = EmployeeRoleId.Create(validGuid).Value;

        // Act
        var stringRepresentation = employeeRoleId.ToString();

        // Assert
        Assert.Equal(validGuid.ToString(), stringRepresentation);
    }

    [Fact]
    public void EmployeeRoleId_Equality_SameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var employeeRoleId1 = EmployeeRoleId.Create(guid).Value;
        var employeeRoleId2 = EmployeeRoleId.Create(guid).Value;

        // Assert
        Assert.Equal(employeeRoleId1, employeeRoleId2);
        Assert.True(employeeRoleId1 == employeeRoleId2);
    }

    [Fact]
    public void EmployeeRoleId_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var employeeRoleId1 = EmployeeRoleId.Create(guid1).Value;
        var employeeRoleId2 = EmployeeRoleId.Create(guid2).Value;

        // Assert
        Assert.NotEqual(employeeRoleId1, employeeRoleId2);
        Assert.True(employeeRoleId1 != employeeRoleId2);
    }
}