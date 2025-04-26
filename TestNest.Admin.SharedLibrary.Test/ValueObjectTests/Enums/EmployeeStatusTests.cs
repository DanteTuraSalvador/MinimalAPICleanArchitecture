using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests.Enums;

public class EmployeeStatusTests
{
    [Fact]
    public void ValidateTransition_ValidTransition_ReturnsSuccess()
    {
        // Arrange
        var currentStatus = EmployeeStatus.Active;
        var nextStatus = EmployeeStatus.Inactive;

        // Act
        var result = EmployeeStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ValidateTransition_InvalidTransition_ReturnsFailureWithInvalidTransitionError()
    {
        // Arrange
        var currentStatus = EmployeeStatus.Inactive;
        var nextStatus = EmployeeStatus.Suspended;

        // Act
        var result = EmployeeStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EmployeeStatusException.ErrorCode.InvalidStatusTransition), result.Errors.First().Code);
        Assert.Equal(EmployeeStatusException.InvalidStatusTransition(currentStatus, nextStatus).Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateTransition_NullCurrentStatus_ReturnsFailureWithNullStatusError()
    {
        // Arrange
        EmployeeStatus currentStatus = null;
        var nextStatus = EmployeeStatus.Active;

        // Act
        var result = EmployeeStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EmployeeStatusException.ErrorCode.NullStatus), result.Errors.First().Code);
        Assert.Equal(EmployeeStatusException.NullStatus().Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateTransition_NullNextStatus_ReturnsFailureWithNullStatusError()
    {
        // Arrange
        var currentStatus = EmployeeStatus.Active;
        EmployeeStatus nextStatus = null;

        // Act
        var result = EmployeeStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EmployeeStatusException.ErrorCode.NullStatus), result.Errors.First().Code);
        Assert.Equal(EmployeeStatusException.NullStatus().Message, result.Errors.First().Message);
    }

    [Fact]
    public void FromId_ValidId_ReturnsSuccessWithValue()
    {
        // Arrange
        var activeId = 0;

        // Act
        var result = EmployeeStatus.FromId(activeId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EmployeeStatus.Active, result.Value);
    }

    [Fact]
    public void FromId_InvalidId_ReturnsFailureWithUnknownStatusIdError()
    {
        // Arrange
        var invalidId = 99;

        // Act
        var result = EmployeeStatus.FromId(invalidId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EmployeeStatusException.ErrorCode.UnknownStatusId), result.Errors.First().Code);
        Assert.Equal(EmployeeStatusException.UnknownStatusId(invalidId).Message, result.Errors.First().Message);
    }

    [Fact]
    public void FromName_ValidName_ReturnsSuccessWithValue()
    {
        // Arrange
        var inactiveName = "Inactive";

        // Act
        var result = EmployeeStatus.FromName(inactiveName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EmployeeStatus.Inactive, result.Value);
    }

    [Fact]
    public void FromName_InvalidName_ReturnsFailureWithUnknownStatusNameError()
    {
        // Arrange
        var invalidName = "DoesNotExist";

        // Act
        var result = EmployeeStatus.FromName(invalidName);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(((int)EmployeeStatusException.ErrorCode.UnknownStatusName).ToString(), result.Errors.First().Code);
        Assert.Equal(EmployeeStatusException.UnknownStatusName(invalidName).Message, result.Errors.First().Message);
    }

    [Fact]
    public void All_ReturnsAllEmployeeStatuses()
    {
        // Act
        var allStatuses = EmployeeStatus.All;

        // Assert
        Assert.Equal(4, allStatuses.Count);
        Assert.Contains(EmployeeStatus.None, allStatuses);
        Assert.Contains(EmployeeStatus.Active, allStatuses);
        Assert.Contains(EmployeeStatus.Inactive, allStatuses);
        Assert.Contains(EmployeeStatus.Suspended, allStatuses);
    }

    [Fact]
    public void EqualityOperator_SameInstances_ReturnsTrue()
    {
        // Arrange
        var active1 = EmployeeStatus.Active;
        var active2 = EmployeeStatus.Active;

        // Assert
        Assert.True(active1 == active2);
    }

    [Fact]
    public void EqualityOperator_DifferentInstancesSameId_ReturnsTrue()
    {
        // Arrange
        var active1 = EmployeeStatus.FromId(0).Value;
        var active2 = EmployeeStatus.FromId(0).Value;

        // Assert
        Assert.True(active1 == active2);
    }

    [Fact]
    public void EqualityOperator_DifferentInstancesDifferentId_ReturnsFalse()
    {
        // Arrange
        var active = EmployeeStatus.Active;
        var inactive = EmployeeStatus.Inactive;

        // Assert
        Assert.False(active == inactive);
    }

    [Fact]
    public void InequalityOperator_SameInstances_ReturnsFalse()
    {
        // Arrange
        var active1 = EmployeeStatus.Active;
        var active2 = EmployeeStatus.Active;

        // Assert
        Assert.False(active1 != active2);
    }

    [Fact]
    public void InequalityOperator_DifferentInstancesSameId_ReturnsFalse()
    {
        // Arrange
        var active1 = EmployeeStatus.FromId(0).Value;
        var active2 = EmployeeStatus.FromId(0).Value;

        // Assert
        Assert.False(active1 != active2);
    }

    [Fact]
    public void InequalityOperator_DifferentInstancesDifferentId_ReturnsTrue()
    {
        // Arrange
        var active = EmployeeStatus.Active;
        var inactive = EmployeeStatus.Inactive;

        // Assert
        Assert.True(active != inactive);
    }
}