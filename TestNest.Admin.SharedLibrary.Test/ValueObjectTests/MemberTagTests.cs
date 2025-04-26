using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class MemberTagTests
{
    public static IEnumerable<object[]> ValidTags => new List<object[]>
    {
        new object[] { "ValidTag" },
        new object[] { "Tag With Spaces" },
        new object[] { "123" },
        new object[] { "A1 B2 C3" },
        new object[] { new string('A', 3) },
        new object[] { new string('A', 100) }
    };

    public static IEnumerable<object[]> InvalidTagData => new List<object[]>
    {
        //new object[] { null, "Tag cannot be null." },
        //new object[] { "", "Tag cannot be empty." },
        //new object[] { " ", "Tag cannot be empty." },
        //new object[] { "Inv@lid", "Tag can only contain letters, numbers, and spaces." },
        new object[] { "ab", "Tag must be between 3 and 100 characters long." },
        new object[] { new string('A', 110), "Tag must be between 3 and 100 characters long." }
    };

    [Theory]
    [MemberData(nameof(ValidTags))]
    public void Create_ValidTag_ReturnsSuccessResult(string tag)
        => Assert.True(MemberTag.Create(tag).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidTagData))]
    public void Create_InvalidTag_ReturnsFailureResultWithValidationError(string tag, string expectedErrorMessage)
    {
        var result = MemberTag.Create(tag);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyMemberTagInstance()
        => Assert.True(MemberTag.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(MemberTag.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(MemberTag.Create("Valid").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidTags))]
    public void WithTag_WithNewValidTag_ReturnsSuccessResultWithUpdatedTag(string newTag)
         => Assert.True(MemberTag.Create("Initial").Value.WithTag(newTag).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidTagData))]
    public void WithTag_WithNewInvalidTag_ReturnsFailureResultWithValidationError(string newTag, string expectedErrorMessage)
    {
        var initialTagResult = MemberTag.Create("Initial");
        Assert.True(initialTagResult.IsSuccess);
        var memberTagInstance = initialTagResult.Value; // Get the instance

        var result = memberTagInstance.WithTag(newTag); // Call WithTag on the instance

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void ToString_ReturnsMemberTagString()
        => Assert.Equal("Test", MemberTag.Create("Test").Value.ToString());
}
