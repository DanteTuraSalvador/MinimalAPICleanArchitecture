using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class SocialMediaAccountNameTests
{
    public static IEnumerable<object[]> ValidAccountNames => new List<object[]>
    {
        new object[] { "valid_user" },
        new object[] { "user.name" },
        new object[] { "user123" },
        new object[] { "a_b.c" },
        new object[] { new string('a', 3) },
        new object[] { new string('z', 50) }
    };

    public static IEnumerable<object[]> InvalidAccountNameData => new List<object[]>
    {
        new object[] { null, "Account name cannot be null." },
        new object[] { "", "Account name cannot be empty." },
        new object[] { " ", "Account name cannot be empty." },
        new object[] { "invalid-char", "Account name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
        new object[] { "user!", "Account name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
        new object[] { "#hashtag", "Account name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
        new object[] { "ab", "Account name must be between 3 and 50 characters long." },
        new object[] { new string('a', 51), "Account name must be between 3 and 50 characters long." }
    };

    [Theory]
    [MemberData(nameof(ValidAccountNames))]
    public void Create_ValidAccountName_ReturnsSuccessResult(string accountName)
        => Assert.True(SocialMediaAccountName.Create(accountName).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidAccountNameData))]
    public void Create_InvalidAccountName_ReturnsFailureResultWithValidationError(string accountName, string expectedErrorMessage)
    {
        var result = SocialMediaAccountName.Create(accountName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptySocialMediaAccountNameInstance()
        => Assert.True(SocialMediaAccountName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(SocialMediaAccountName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(SocialMediaAccountName.Create("valid").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidAccountNames))]
    public void Update_WithNewValidAccountName_ReturnsSuccessResultWithUpdatedName(string newName)
    {
        var initialName = SocialMediaAccountName.Create("old_name").Value;
        Assert.True(initialName.Update(newName).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidAccountNameData))]
    public void Update_WithNewInvalidAccountName_ReturnsFailureResultWithValidationError(string newName, string expectedErrorMessage)
    {
        var initialName = SocialMediaAccountName.Create("old_name").Value;
        var result = initialName.Update(newName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void ToString_ReturnsAccountNameString()
        => Assert.Equal("test_account", SocialMediaAccountName.Create("test_account").Value.ToString());
}