using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class PersonNameTests
{
    public static IEnumerable<object[]> ValidNames => new List<object[]>
    {
        new object[] { "John", null, "Doe" },
        new object[] { "Mary", "Anne", "Smith" },
        new object[] { "Jean-Paul", null, "Sartre" },
        new object[] { "O'Malley", null, "Fitzgerald" },
        new object[] { "Kim", null, "Min-ji" },
        new object[] { new string('A', 2), null, new string('B', 2) },
        new object[] { new string('C', 100), new string('D', 50), new string('E', 100) }
    };

    public static IEnumerable<object[]> InvalidNameData => new List<object[]>
    {
        new object[] { null, null, "Doe", "Person name is invalid" }, // Null FirstName
        new object[] { "John", null, null, "Person name is invalid" }, // Null LastName
        new object[] { "", null, "Doe", "First name cannot be empty." },
        new object[] { "John", null, "", "Last name cannot be empty." },
        new object[] { "John!", null, "Doe", "Names can only contain letters, apostrophes, and hyphens." },
        new object[] { "John", "A1", "Doe", "Names can only contain letters, apostrophes, and hyphens." },
        new object[] { "John", null, "Doe#", "Names can only contain letters, apostrophes, and hyphens." },
        new object[] { "J", null, "Doe", "Names must be between 2 and 100 characters long." },
        new object[] { new string('A', 101), null, "Doe", "Names must be between 2 and 100 characters long." },
        new object[] { "John", null, "D", "Names must be between 2 and 100 characters long." },
        new object[] { "John", null, new string('B', 101), "Names must be between 2 and 100 characters long." },
        new object[] { "John", "A", "Doe", "Names must be between 2 and 100 characters long." },
        new object[] { "John", new string('B', 101), "Doe", "Names must be between 2 and 100 characters long." }
    };

    [Theory]
    [MemberData(nameof(ValidNames))]
    public void Create_ValidPersonName_ReturnsSuccessResult(string firstName, string? middleName, string lastName)
        => Assert.True(PersonName.Create(firstName, middleName, lastName).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidNameData))]
    public void Create_InvalidPersonName_ReturnsFailureResultWithValidationError(string firstName, string? middleName, string lastName, string expectedErrorMessage)
    {
        var result = PersonName.Create(firstName, middleName, lastName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyPersonNameInstance()
        => Assert.True(PersonName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(PersonName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(PersonName.Create("John", null, "Doe").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidNames))]
    public void Update_WithNewValidName_ReturnsSuccessResultWithUpdatedName(string firstName, string? middleName, string lastName)
    {
        var initialName = PersonName.Create("Old", null, "Name").Value;
        Assert.True(initialName.Update(firstName, middleName, lastName).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidNameData))]
    public void Update_WithNewInvalidName_ReturnsFailureResultWithValidationError(string firstName, string? middleName, string lastName, string expectedErrorMessage)
    {
        var initialName = PersonName.Create("Old", null, "Name").Value;
        var result = initialName.Update(firstName, middleName, lastName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void GetFullName_WithMiddleName_ReturnsFullName()
        => Assert.Equal("John Anne Doe", PersonName.Create("John", "Anne", "Doe").Value.GetFullName());

    [Fact]
    public void GetFullName_WithoutMiddleName_ReturnsFirstNameLastName()
        => Assert.Equal("John Doe", PersonName.Create("John", null, "Doe").Value.GetFullName());

    [Fact]
    public void ToString_ReturnsFullName()
        => Assert.Equal("John Doe", PersonName.Create("John", null, "Doe").Value.ToString());
}