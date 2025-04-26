using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Tests.ValueObjects;

public class EmployeeNumberTests
{
    [Theory]
    [InlineData(null, EmployeeNumberException.ErrorCode.NullEmployeeNumber)]
    [InlineData("", EmployeeNumberException.ErrorCode.EmptyEmployeeNumber)]
    [InlineData("   ", EmployeeNumberException.ErrorCode.EmptyEmployeeNumber)]
    public void Create_InvalidInput_ReturnsFailureWithCorrectError(
        string input,
        EmployeeNumberException.ErrorCode expectedErrorCode)
    {
        // Act
        var result = EmployeeNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
        var expectedException = CreateExpectedException(expectedErrorCode);
        Assert.Contains(result.Errors, e =>
            e.Message == expectedException.Message);
    }

    [Theory]
    [InlineData("A1!")]
    [InlineData("AB#")]
    [InlineData("123$")]
    public void Create_InvalidFormat_ReturnsInvalidFormatError(string input)
    {
        // Act
        var result = EmployeeNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
        var expectedException = EmployeeNumberException.InvalidEmployeeNumberFormat();
        Assert.Contains(result.Errors, e =>
            e.Message == expectedException.Message);
    }

    [Theory]
    [InlineData("AB")]    // Too short
    [InlineData("ABCDEFGHIJK")]  // Too long
    public void Create_InvalidLength_ReturnsLengthError(string input)
    {
        // Act
        var result = EmployeeNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
        var expectedException = EmployeeNumberException.LengthOutOfRangeEmployeeNumber();
        Assert.Contains(result.Errors, e =>
            e.Message == expectedException.Message);
    }

    [Theory]
    [InlineData("A-1")]
    [InlineData("ABC")]
    [InlineData("123")]
    [InlineData("A1B2C3")]
    [InlineData("ABCDEFGHIJ")]  // 10 characters
    public void Create_ValidInput_ReturnsSuccess(string input)
    {
        // Act
        var result = EmployeeNumber.Create(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value.EmployeeNo);
    }

    [Fact]
    public void Empty_ReturnsEmptyInstance()
    {
        // Act
        var empty1 = EmployeeNumber.Empty();
        var empty2 = EmployeeNumber.Empty();

        // Assert
        Assert.True(empty1.IsEmpty());
        Assert.Same(empty1, empty2);
        Assert.Equal(string.Empty, empty1.EmployeeNo);
    }

    [Fact]
    public void WithRoleName_ValidInput_ReturnsNewInstance()
    {
        // Arrange
        var original = EmployeeNumber.Create("ABC-123").Value;
        const string newNumber = "XYZ-789";

        // Act
        var result = original.WithRoleName(newNumber);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newNumber, result.Value.EmployeeNo);
        Assert.NotSame(original, result.Value);
    }

    [Fact]
    public void ToString_ReturnsEmployeeNumber()
    {
        // Arrange
        const string expected = "EMP-123";
        var employeeNumber = EmployeeNumber.Create(expected).Value;

        // Act & Assert
        Assert.Equal(expected, employeeNumber.ToString());
    }

    private EmployeeNumberException CreateExpectedException(EmployeeNumberException.ErrorCode errorCode)
    {
        return errorCode switch
        {
            EmployeeNumberException.ErrorCode.EmptyEmployeeNumber =>
                EmployeeNumberException.EmptyEmployeeNumber(),
            EmployeeNumberException.ErrorCode.InvalidEmployeeNumberFormat =>
                EmployeeNumberException.InvalidEmployeeNumberFormat(),
            EmployeeNumberException.ErrorCode.LengthOutOfRangeEmployeeNumber =>
                EmployeeNumberException.LengthOutOfRangeEmployeeNumber(),
            EmployeeNumberException.ErrorCode.NullEmployeeNumber =>
                EmployeeNumberException.NullEmployeeNumber(),
            _ => throw new System.NotImplementedException()
        };
    }
}