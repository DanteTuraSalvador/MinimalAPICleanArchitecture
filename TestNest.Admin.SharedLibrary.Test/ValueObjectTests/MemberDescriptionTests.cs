using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class MemberDescriptionTests
{
    public static IEnumerable<object[]> ValidDescriptions => new List<object[]>
        {
            new object[] { "This is a valid description." },
            new object[] { "A slightly longer description that fits within the length constraints." },
            new object[] { "Exactly5" },
            new object[] { new string('A', 500) } // Max allowed length
        };

    public static IEnumerable<object[]> InvalidLengthDescriptions => new List<object[]>
        {
            new object[] { "abcd" }, // Too short (4 chars)
            new object[] { new string('A', 501) } // Too long (501 chars)
        };

    [Theory]
    [MemberData(nameof(ValidDescriptions))]
    public void Create_ValidDescription_ReturnsSuccessResult(string description)
    {
        // Act
        var result = MemberDescription.Create(description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public void Create_NullDescription_ReturnsFailureResultWithNullError()
    {
        // Act
        var result = MemberDescription.Create(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error =>
            error.Code == MemberDescriptionException.ErrorCode.NullEstablishmentMemberDescription.ToString() &&
            error.Message == "Establishment member cannot be null.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_EmptyOrWhiteSpaceDescription_ReturnsFailureResultWithEmptyError(string description)
    {
        // Act
        var result = MemberDescription.Create(description);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error =>
            error.Code == MemberDescriptionException.ErrorCode.EmptyDescription.ToString() &&
            error.Message == "Description cannot be empty.");
    }

    [Theory]
    [MemberData(nameof(InvalidLengthDescriptions))]
    public void Create_OutOfRangeDescriptionLength_ReturnsFailureResultWithLengthOutOfRangeError(string description)
    {
        // Act
        var result = MemberDescription.Create(description);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error =>
            error.Code == MemberDescriptionException.ErrorCode.LengthOutOfRange.ToString() &&
            error.Message == "Description must be between 5 and 500 characters long.");
    }
}
