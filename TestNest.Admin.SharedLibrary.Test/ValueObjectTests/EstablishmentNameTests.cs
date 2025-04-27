using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class EstablishmentNameTests
{
    [Theory]
    [InlineData("The Cozy Cafe")]
    [InlineData("ABC 123 & Co.")]
    [InlineData("Mike's Pizzeria, Inc.")]
    [InlineData("O'Malley's Pub'n Grub")]
    [InlineData("Dr. Smith's Clinic-Annex")]
    public void Create_ValidName_ReturnsSuccessResult(string name)
    {
        // Act
        var result = EstablishmentName.Create(name);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
    }

    [Fact]
    public void Create_NullName_ReturnsFailureResultWithNullError()
    {
        // Act
        var result = EstablishmentName.Create(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == EstablishmentNameException.ErrorCode.NullEstablishmentName.ToString() && error.Message == "Establishment name cannot be null.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_EmptyOrWhiteSpaceName_ReturnsFailureResultWithEmptyError(string name)
    {
        // Act
        var result = EstablishmentName.Create(name);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == EstablishmentNameException.ErrorCode.EmptyName.ToString() && error.Message == "Establishment name cannot be empty.");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("This is a very very very very very very very very very very long establishment name that exceeds the maximum allowed length")]
    public void Create_InvalidLengthName_ReturnsFailureResultWithInvalidLengthError(string name)
    {
        // Act
        var result = EstablishmentName.Create(name);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == EstablishmentNameException.ErrorCode.InvalidLength.ToString() && error.Message == "Establishment name must be between 3 and 50 characters.");
    }

    [Theory]
    [InlineData("Invalid!")]
    [InlineData("#Hashtag")]
    [InlineData("$DollarStore")]
    [InlineData("%Percent")]
    [InlineData("*Asterisk")]
    [InlineData("(OpenParen")]
    [InlineData(")CloseParen")]
    [InlineData("=Equals")]
    [InlineData("+Plus")]
    [InlineData("[OpenBracket")]
    [InlineData("]CloseBracket")]
    [InlineData("\\Backslash")]
    [InlineData("|Pipe")]
    [InlineData(";Semicolon")]
    [InlineData(":Colon")]
    [InlineData("<LessThan")]
    [InlineData(">GreaterThan")]
    [InlineData("?QuestionMark")]
    [InlineData("/ForwardSlash")]
    [InlineData("/ForwardSlash")]
    [InlineData("`Grave")]
    [InlineData("~Tilde")]
    [InlineData("@AtSign")]
    public void Create_InvalidCharactersName_ReturnsFailureResultWithInvalidCharactersError(string name)
    {
        // Act
        var result = EstablishmentName.Create(name);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == EstablishmentNameException.ErrorCode.InvalidCharacters.ToString() && error.Message == "Establishment name contains invalid characters.");
    }

    [Fact]
    public void Empty_ReturnsAnEmptyEstablishmentNameInstance()
    {
        // Act
        var emptyName = EstablishmentName.Empty();

        // Assert
        Assert.True(emptyName.IsEmpty());
        Assert.Equal(string.Empty, emptyName.Name);
    }

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
    {
        // Arrange & Act
        var emptyName = EstablishmentName.Empty();

        // Assert
        Assert.True(emptyName.IsEmpty());
    }

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
    {
        // Arrange
        var establishmentName = EstablishmentName.Create("Valid Name").Value;

        // Act & Assert
        Assert.False(establishmentName.IsEmpty());
    }

    [Theory]
    [InlineData("Old Name", "New Name")]
    [InlineData("Cafe A", "Cafe B & C")]
    public void Update_WithNewValidName_ReturnsSuccessResultWithUpdatedName(string initialName, string newName)
    {
        // Arrange
        var establishmentName = EstablishmentName.Create(initialName).Value;

        // Act
        var result = establishmentName.Update(newName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, result.Value.Name);
    }

    [Theory]
    [InlineData("Valid", null)]
    [InlineData("Valid", "")]
    [InlineData("Valid", "  ")]
    [InlineData("Valid", "Inv@lid")]
    [InlineData("Valid", "Too long establishment name exceeding the limit1234")]
    public void Update_WithNewInvalidName_ReturnsFailureResultWithValidationError(string initialName, string newName)
    {
        // Arrange
        var establishmentName = EstablishmentName.Create(initialName).Value;

        // Act
        var result = establishmentName.Update(newName);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ToString_ReturnsEstablishmentNameString()
    {
        // Arrange
        var establishmentName = EstablishmentName.Create("Test Name").Value;

        // Act
        var toStringResult = establishmentName.ToString();

        // Assert
        Assert.Equal("Test Name", toStringResult);
    }
}
