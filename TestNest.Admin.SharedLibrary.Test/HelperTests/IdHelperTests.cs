using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.Test.HelperTests;

public class IdHelperTests
{
    [Fact]
    public void ValidateAndCreateId_ValidGuidString_ReturnsSuccessResultWithId()
    {
        // Arrange
        var validGuidString = Guid.NewGuid().ToString();

        // Act
        var result = IdHelper.ValidateAndCreateId<TestId>(validGuidString);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Guid.Parse(validGuidString), result.Value.Value);
    }

    [Fact]
    public void ValidateAndCreateId_InvalidGuidString_ReturnsFailureResultWithInvalidGuidError()
    {
        // Arrange
        var invalidGuidString = "not-a-guid";

        // Act
        var result = IdHelper.ValidateAndCreateId<TestId>(invalidGuidString);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(IdHelper.InvalidGuidFormatErrorCode, result.Errors.First().Code);
        Assert.Equal(IdValidationException.InvalidGuidFormat().Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateAndCreateId_EmptyGuidString_ReturnsFailureResultWithNullIdErrorFromCreateMethod()
    {
        // Arrange
        var emptyGuidString = Guid.Empty.ToString();

        // Act
        var result = IdHelper.ValidateAndCreateId<TestId>(emptyGuidString);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(StronglyTypedIdException.ErrorCode.NullId.ToString(), result.Errors.First().Code);
        Assert.Equal(StronglyTypedIdException.NullId().Message, result.Errors.First().Message);
    }

    [Fact]
    public void ValidateAndCreateId_NonExistentCreateMethod_ReturnsFailureResultWithCreateMethodNotFoundError()
    {
        // Arrange
        var validGuidString = Guid.NewGuid().ToString();

        // Act
        var result = IdHelper.ValidateAndCreateId<TestIdWithoutCreate>(validGuidString);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.ErrorType);
        Assert.Single(result.Errors);
        Assert.Equal(IdHelper.CreateMethodNotFoundErrorCode, result.Errors.First().Code);
        Assert.Equal($"Create method not found for {typeof(TestIdWithoutCreate).Name}.", result.Errors.First().Message);
    }

    // Helper class for testing
    private sealed record TestId(Guid Value) : StronglyTypedId<TestId>(Value)
    {
        public static Result<TestId> Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                var exception = StronglyTypedIdException.NullId();
                return Result<TestId>.Failure(ErrorType.Validation,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
            return Result<TestId>.Success(new TestId(value));
        }
    }

    // Helper class without a static Create method for testing
    private sealed record TestIdWithoutCreate(Guid Value) : StronglyTypedId<TestIdWithoutCreate>(Value);
}