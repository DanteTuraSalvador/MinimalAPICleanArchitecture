using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;

namespace TestNest.Admin.SharedLibrary.Test.Common.Guards;

public class GuardTests
{
    private static Exception CreateTestException() => new InvalidOperationException("Test exception");

    [Fact]
    public void Against_WithValidCondition_ReturnsSuccess()
    {
        // Arrange
        bool validation() => true;

        // Act
        var result = Guard.Against(validation, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Against_WithInvalidCondition_ReturnsFailure()
    {
        // Arrange
        bool validation() => false;

        // Act
        var result = Guard.Against(validation, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);

        // Check if the error contains the expected exception type
        var firstError = result.Errors.First();

        // Option 2: If Error has a Message property
        Assert.Equal("Test exception", firstError.Message);
    }

    [Fact]
    public void Against_WithThrowingValidation_ReturnsFailureWithWrappedException()
    {
        // Arrange
        bool validation() => throw new ArgumentException("Inner exception");

        // Act
        var result = Guard.Against(validation, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);

        // Option 2: If the error has the message but not the exception
        Assert.Contains("Validation failed", result.Errors.First().Message);

        // Option 3: If the error wraps the exception in some other way
        // You'll need to inspect your Error class to see how it stores the inner exception
    }

    [Fact]
    public void AgainstNull_WithNullValue_ReturnsFailure()
    {
        // Arrange
        object nullObject = null;

        // Act
        var result = Guard.AgainstNull(nullObject, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstNull_WithNonNullValue_ReturnsSuccess()
    {
        // Arrange
        var validObject = new object();

        // Act
        var result = Guard.AgainstNull(validObject, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpace_WithInvalidValues_ReturnsFailure(string value)
    {
        // Act
        var result = Guard.AgainstNullOrWhiteSpace(value, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstNullOrWhiteSpace_WithValidValue_ReturnsSuccess()
    {
        // Arrange
        var validString = "valid";

        // Act
        var result = Guard.AgainstNullOrWhiteSpace(validString, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstLength_WithCorrectLength_ReturnsSuccess()
    {
        // Arrange
        var value = "12345";
        var requiredLength = 5;

        // Act
        var result = Guard.AgainstLength(value, requiredLength, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstLength_WithIncorrectLength_ReturnsFailure()
    {
        // Arrange
        var value = "12345";
        var requiredLength = 4;

        // Act
        var result = Guard.AgainstLength(value, requiredLength, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstRegex_WithMatchingPattern_ReturnsSuccess()
    {
        // Arrange
        var value = "test@example.com";
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Act
        var result = Guard.AgainstRegex(value, pattern, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstRegex_WithNonMatchingPattern_ReturnsFailure()
    {
        // Arrange
        var value = "invalid-email";
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Act
        var result = Guard.AgainstRegex(value, pattern, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstInvalidInput_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var value = 10;
        bool validation(int x) => x > 5;

        // Act
        var result = Guard.AgainstInvalidInput(value, validation, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstInvalidInput_WithInvalidInput_ReturnsFailure()
    {
        // Arrange
        var value = 3;
        bool validation(int x) => x > 5;

        // Act
        var result = Guard.AgainstInvalidInput(value, validation, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(5, 1, 10)]
    [InlineData(1, 1, 10)]
    [InlineData(10, 1, 10)]
    public void AgainstRange_WithValueInRange_ReturnsSuccess(int value, int min, int max)
    {
        // Act
        var result = Guard.AgainstRange(value, min, max, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    public void AgainstRange_WithValueOutOfRange_ReturnsFailure(int value, int min, int max)
    {
        // Act
        var result = Guard.AgainstRange(value, min, max, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void AgainstNegative_WithNonNegativeValue_ReturnsSuccess(decimal value)
    {
        // Act
        var result = Guard.AgainstNegative(value, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstNegative_WithNegativeValue_ReturnsFailure()
    {
        // Arrange
        var value = -0.1m;

        // Act
        var result = Guard.AgainstNegative(value, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstNonPositive_WithPositiveValue_ReturnsSuccess()
    {
        // Arrange
        var value = 0.1m;

        // Act
        var result = Guard.AgainstNonPositive(value, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgainstNonPositive_WithNonPositiveValue_ReturnsFailure(decimal value)
    {
        // Act
        var result = Guard.AgainstNonPositive(value, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstEmptyCollection_WithNonEmptyCollection_ReturnsSuccess()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };

        // Act
        var result = Guard.AgainstEmptyCollection(collection, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstEmptyCollection_WithEmptyCollection_ReturnsFailure()
    {
        // Arrange
        var collection = new List<int>();

        // Act
        var result = Guard.AgainstEmptyCollection(collection, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstEmptyCollection_WithNullCollection_ReturnsFailure()
    {
        // Arrange
        List<int> collection = null;

        // Act
        var result = Guard.AgainstEmptyCollection(collection, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstPastDate_WithFutureDate_ReturnsSuccess()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Guard.AgainstPastDate(futureDate, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstPastDate_WithPastDate_ReturnsFailure()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = Guard.AgainstPastDate(pastDate, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstFutureDate_WithPastDate_ReturnsSuccess()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = Guard.AgainstFutureDate(pastDate, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstFutureDate_WithFutureDate_ReturnsFailure()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Guard.AgainstFutureDate(futureDate, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AgainstCondition_WithFalseCondition_ReturnsSuccess()
    {
        // Arrange
        var invalidCondition = false;

        // Act
        var result = Guard.AgainstCondition(invalidCondition, CreateTestException);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AgainstCondition_WithTrueCondition_ReturnsFailure()
    {
        // Arrange
        var invalidCondition = true;

        // Act
        var result = Guard.AgainstCondition(invalidCondition, CreateTestException);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Aggregate_WithAllSuccessResults_ReturnsSuccess()
    {
        // Arrange
        var results = new[]
        {
                Result.Success(),
                Result.Success(),
                Result.Success()
            };

        // Act
        var result = Guard.Aggregate(results);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Aggregate_WithAnyFailure_ReturnsFailure()
    {
        // Arrange
        var results = new[]
        {
                Result.Success(),
                Result.Failure(CreateTestException()),
                Result.Success()
            };

        // Act
        var result = Guard.Aggregate(results);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Aggregate_WithMultipleFailures_ReturnsCombinedFailure()
    {
        // Arrange
        var results = new[]
        {
                Result.Failure(CreateTestException()),
                Result.Failure(new ArgumentException("Arg exception")),
                Result.Success()
            };

        // Act
        var result = Guard.Aggregate(results);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count());
    }
}
