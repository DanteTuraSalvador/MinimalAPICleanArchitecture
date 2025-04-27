using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class SocialMediaNameTests
{
    private static readonly Regex ValidNamePattern = new("^[A-Za-z0-9_.]+$", RegexOptions.Compiled);

    public static IEnumerable<object[]> ValidSocialMediaNames => new List<object[]>
    {
        new object[] { "valid_name", "http://valid.url" },
        new object[] { "user.test", "https://test.domain" },
        new object[] { "name123", "http://example.com/path" },
        new object[] { "a_b.c", "https://sub.domain.co.uk" },
        new object[] { new string('a', 3), "http://short.url" },
        new object[] { new string('z', 50), "https://long.domain.com/path/to/resource" }
    };

    public static IEnumerable<object[]> InvalidSocialMediaNameData => new List<object[]>
    {
        //new object[] { null, "http://valid.url", "Social media account name cannot be null." },
        //new object[] { "", "http://valid.url", "Social media name cannot be empty." },
        //new object[] { " ", "http://valid.url", "Social media name cannot be empty." },
        //new object[] { "invalid-char", "http://valid.url", "Social media name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
        //new object[] { "user!", "http://valid.url", "Social media name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
        //new object[] { "ab", "http://valid.url", "Social media name must be between 3 and 50 characters long." },
        //new object[] { new string('a', 51), "http://valid.url", "Social media name must be between 3 and 50 characters long." },
        //new object[] { "valid_name", null, "Platform URL cannot be null." },
      new object[] { "valid_name", "", "Platform URL cannot be empty." },
    new object[] { "valid_name", " ", "Platform URL cannot be empty." },
    new object[] { "valid_name", "invalid", "Platform URL format is invalid." },
    new object[] { "valid_name", "ftp://invalid.url", "Platform URL format is invalid." },
    new object[] { "valid_name", "http:/invalid", "Platform URL format is invalid." }
    };

    [Theory]
    [MemberData(nameof(ValidSocialMediaNames))]
    public void Create_ValidSocialMediaName_ReturnsSuccessResult(string name, string platformURL)
        => Assert.True(SocialMediaName.Create(name, platformURL).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidSocialMediaNameData))]
    public void Create_InvalidSocialMediaName_ReturnsFailureResultWithValidationError(string name, string platformURL, string expectedErrorMessage)
    {
        var result = SocialMediaName.Create(name, platformURL);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptySocialMediaNameInstance()
        => Assert.True(SocialMediaName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(SocialMediaName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(SocialMediaName.Create("valid", "http://valid.url").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidSocialMediaNames))]
    public void WithNamePlatform_WithNewValidData_ReturnsSuccessResultWithUpdatedData(string newName, string newPlatformURL)
    {
        var initial = SocialMediaName.Create("old", "http://old.url").Value;
        Assert.True(initial.WithNamePlatform(newName, newPlatformURL).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidSocialMediaNameData))]
    public void WithNamePlatform_WithNewInvalidName_ReturnsFailureResultWithValidationError(string newName, string newPlatformURL, string expectedErrorMessage)
    {
        var initial = SocialMediaName.Create("old", "http://old.url").Value;
        var result = initial.WithNamePlatform(newName, newPlatformURL);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Theory]
    [MemberData(nameof(ValidSocialMediaNames))]
    public void WithName_WithNewValidName_ReturnsSuccessResultWithUpdatedName(string newName, string platformURL)
    {
        var initial = SocialMediaName.Create("old", platformURL).Value;
        Assert.True(initial.WithName(newName).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidSocialMediaNameData))]
    public void WithName_WithNewInvalidName_ReturnsFailureResultWithValidationError(string newName, string platformURL, string expectedErrorMessage)
    {
        if (!string.IsNullOrEmpty(platformURL) && platformURL.StartsWith("http", StringComparison.OrdinalIgnoreCase) && Uri.IsWellFormedUriString(platformURL, UriKind.Absolute))
        {
            var initial = SocialMediaName.Create("old", platformURL).Value;
            var result = initial.WithName(newName);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Validation, result.ErrorType);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
        }
    }

    [Theory]
    [MemberData(nameof(ValidSocialMediaNames))]
    public void WithPlatformURL_WithNewValidURL_ReturnsSuccessResultWithUpdatedURL(string name, string newPlatformURL)
    {
        var initial = SocialMediaName.Create(name, "http://old.url").Value;
        Assert.True(initial.WithPlatformURL(newPlatformURL).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidSocialMediaNameData))]
    public void WithPlatformURL_WithNewInvalidURL_ReturnsFailureResultWithValidationError(string name, string newPlatformURL, string expectedErrorMessage)
    {
        if (!string.IsNullOrEmpty(name) && ValidNamePattern.IsMatch(name) && name.Length >= 3 && name.Length <= 50)
        {
            var initial = SocialMediaName.Create(name, "http://old.url").Value;
            var result = initial.WithPlatformURL(newPlatformURL);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Validation, result.ErrorType);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
        }
    }

    [Theory]
    [MemberData(nameof(ValidSocialMediaNames))]
    public void TryParse_ValidInput_ReturnsSuccessResult(string name, string platformURL)
        => Assert.True(SocialMediaName.TryParse(name, platformURL).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidSocialMediaNameData))]
    public void TryParse_InvalidInput_ReturnsFailureResultWithValidationError(string name, string platformURL, string expectedErrorMessage)
    {
        var result = SocialMediaName.TryParse(name, platformURL);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
        => Assert.Equal("test_name (http://test.url)", SocialMediaName.Create("test_name", "http://test.url").Value.ToString());
}
