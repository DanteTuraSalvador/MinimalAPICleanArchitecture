using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests.Enums;

public class EstablishmentStatusTests
{
    [Fact]
    public void ValidateTransition_ValidTransition_ReturnsSuccess()
    {
        // Arrange
        var currentStatus = EstablishmentStatus.Pending;
        var nextStatus = EstablishmentStatus.Approval;

        // Act
        var result = EstablishmentStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ValidateTransition_InvalidTransition_ReturnsFailureWithInvalidTransitionError()
    {
        // Arrange
        var currentStatus = EstablishmentStatus.Active;
        var nextStatus = EstablishmentStatus.Pending;

        // Act
        var result = EstablishmentStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EstablishmentStatusException.ErrorCode.InvalidStatusTransition), result.Errors.First().Code);
        Assert.Equal(EstablishmentStatusException.InvalidStatusTransition(currentStatus, nextStatus).Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateTransition_NullCurrentStatus_ReturnsFailureWithNullStatusError()
    {
        // Arrange
        EstablishmentStatus currentStatus = null;
        var nextStatus = EstablishmentStatus.Pending;

        // Act
        var result = EstablishmentStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EstablishmentStatusException.ErrorCode.NullStatus), result.Errors.First().Code);
        Assert.Equal(EstablishmentStatusException.NullStatus().Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateTransition_NullNextStatus_ReturnsFailureWithNullStatusError()
    {
        // Arrange
        var currentStatus = EstablishmentStatus.Pending;
        EstablishmentStatus nextStatus = null;

        // Act
        var result = EstablishmentStatus.ValidateTransition(currentStatus, nextStatus);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EstablishmentStatusException.ErrorCode.NullStatus), result.Errors.First().Code);
        Assert.Equal(EstablishmentStatusException.NullStatus().Message, result.Errors.First().Message);
    }

    [Fact]
    public void FromId_ValidId_ReturnsSuccessWithValue()
    {
        // Arrange
        var activeId = 3;

        // Act
        var result = EstablishmentStatus.FromId(activeId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstablishmentStatus.Active, result.Value);
    }

    [Fact]
    public void FromId_InvalidId_ReturnsFailureWithUnknownStatusIdError()
    {
        // Arrange
        var invalidId = 99;

        // Act
        var result = EstablishmentStatus.FromId(invalidId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(nameof(EstablishmentStatusException.ErrorCode.UnknownStatusId), result.Errors.First().Code);
        Assert.Equal(EstablishmentStatusException.UnknownStatusId(invalidId).Message, result.Errors.First().Message);
    }

    [Fact]
    public void FromName_ValidName_ReturnsSuccessWithValue()
    {
        // Arrange
        var rejectedName = "Rejected";

        // Act
        var result = EstablishmentStatus.FromName(rejectedName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstablishmentStatus.Rejected, result.Value);
    }

    [Fact]
    public void FromName_InvalidName_ReturnsFailureWithUnknownStatusNameError()
    {
        // Arrange
        var invalidName = "DoesNotExist";

        // Act
        var result = EstablishmentStatus.FromName(invalidName);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(((int)EstablishmentStatusException.ErrorCode.UnknownStatusName).ToString(), result.Errors.First().Code);
        Assert.Equal(EstablishmentStatusException.UnknownStatusName(invalidName).Message, result.Errors.First().Message);
    }

    [Fact]
    public void All_ReturnsAllEstablishmentStatuses()
    {
        // Act
        var allStatuses = EstablishmentStatus.All;

        // Assert
        Assert.Equal(7, allStatuses.Count);
        Assert.Contains(EstablishmentStatus.None, allStatuses);
        Assert.Contains(EstablishmentStatus.Pending, allStatuses);
        Assert.Contains(EstablishmentStatus.Approval, allStatuses);
        Assert.Contains(EstablishmentStatus.Rejected, allStatuses);
        Assert.Contains(EstablishmentStatus.Active, allStatuses);
        Assert.Contains(EstablishmentStatus.InActive, allStatuses);
        Assert.Contains(EstablishmentStatus.Suspended, allStatuses);
    }

    [Fact]
    public void EqualityOperator_SameInstances_ReturnsTrue()
    {
        // Arrange
        var active1 = EstablishmentStatus.Active;
        var active2 = EstablishmentStatus.Active;

        // Assert
        Assert.True(active1 == active2);
    }

    [Fact]
    public void EqualityOperator_DifferentInstancesSameId_ReturnsTrue()
    {
        // Arrange
        var active1 = EstablishmentStatus.FromId(3).Value;
        var active2 = EstablishmentStatus.FromId(3).Value;

        // Assert
        Assert.True(active1 == active2);
    }

    [Fact]
    public void EqualityOperator_DifferentInstancesDifferentId_ReturnsFalse()
    {
        // Arrange
        var active = EstablishmentStatus.Active;
        var pending = EstablishmentStatus.Pending;

        // Assert
        Assert.False(active == pending);
    }

    [Fact]
    public void InequalityOperator_SameInstances_ReturnsFalse()
    {
        // Arrange
        var active1 = EstablishmentStatus.Active;
        var active2 = EstablishmentStatus.Active;

        // Assert
        Assert.False(active1 != active2);
    }

    [Fact]
    public void InequalityOperator_DifferentInstancesSameId_ReturnsFalse()
    {
        // Arrange
        var active1 = EstablishmentStatus.FromId(3).Value;
        var active2 = EstablishmentStatus.FromId(3).Value;

        // Assert
        Assert.False(active1 != active2);
    }

    [Fact]
    public void InequalityOperator_DifferentInstancesDifferentId_ReturnsTrue()
    {
        // Arrange
        var active = EstablishmentStatus.Active;
        var pending = EstablishmentStatus.Pending;

        // Assert
        Assert.True(active != pending);
    }
}
