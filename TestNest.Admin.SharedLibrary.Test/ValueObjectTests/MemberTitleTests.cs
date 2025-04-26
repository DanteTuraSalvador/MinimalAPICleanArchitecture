using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class MemberTitleTests
{
    public static IEnumerable<object[]> ValidTitles => new List<object[]>
    {
        new object[] { "Mr" },
        new object[] { "Ms" },
        new object[] { "Dr" },
        new object[] { "Professor" },
        new object[] { "Sir Reginald" },
        new object[] { new string('A', 2) },
        new object[] { new string('Z', 100) }
    };

    public static IEnumerable<object[]> InvalidTitleData => new List<object[]>
    {
        new object[] { null, "Title cannot be null." },
        new object[] { "", "Title cannot be empty." },
        new object[] { " ", "Title cannot be empty." },
        new object[] { "Mr.", "Title can only contain letters and spaces." },
        new object[] { "Dr.", "Title can only contain letters and spaces." },
        new object[] { "Prof.", "Title can only contain letters and spaces." },
        new object[] { "A", "Title must be between 2 and 100 characters long." },
        new object[] { new string('B', 101), "Title must be between 2 and 100 characters long." }
    };

    [Theory]
    [MemberData(nameof(ValidTitles))]
    public void Create_ValidTitle_ReturnsSuccessResult(string title)
        => Assert.True(MemberTitle.Create(title).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidTitleData))]
    public void Create_InvalidTitle_ReturnsFailureResultWithValidationError(string title, string expectedErrorMessage)
    {
        var result = MemberTitle.Create(title);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyMemberTitleInstance()
        => Assert.True(MemberTitle.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(MemberTitle.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(MemberTitle.Create("Valid").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidTitles))]
    public void WithTitle_WithNewValidTitle_ReturnsSuccessResultWithUpdatedTitle(string newTitle) => Assert.True(MemberTitle.Create("Initial").Value.WithTitle(newTitle).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidTitleData))]
    public void WithTitle_WithNewInvalidTitle_ReturnsFailureResultWithValidationError(string newTitle, string expectedErrorMessage)
    {
        var initialTitleResult = MemberTitle.Create("Initial");
        Assert.True(initialTitleResult.IsSuccess);
        var result = initialTitleResult.Value.WithTitle(newTitle); // Call WithTitle on the instance
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void ToString_ReturnsMemberTitleString()
        => Assert.Equal("Test Title", MemberTitle.Create("Test Title").Value.ToString());
}
