using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class EmailAddressTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@subdomain.example.co.uk")]
    [InlineData("very.long.email.address.with.many.parts@example.university.edu.ph")]
    public void Create_ValidEmail_ReturnsSuccessResultWithEmailAddress(string email)
    {
        // Act
        var result = EmailAddress.Create(email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("user@")]
    [InlineData("@domain")]
    [InlineData("user@domain.")]
    [InlineData(".user@domain")]
    [InlineData("user@@domain")]
    [InlineData("user@domain_invalid")]
    [InlineData("user@.domain")]
    [InlineData("user@-domain")]
    public void Create_InvalidEmail_ReturnsFailureResultWithValidationError(string email)
    {
        // Act
        var result = EmailAddress.Create(email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == typeof(EmailAddressException).Name);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyEmailAddressInstance()
    {
        // Act
        var emptyEmail = EmailAddress.Empty();

        // Assert
        Assert.True(emptyEmail.IsEmpty());
        Assert.Equal(string.Empty, emptyEmail.Email);
    }

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
    {
        // Arrange & Act
        var emptyEmail = EmailAddress.Empty(); // Use the static Empty() method

        // Assert
        Assert.True(emptyEmail.IsEmpty());
    }

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
    {
        // Arrange
        var emailAddress = EmailAddress.Create("test@example.com").Value;

        // Act & Assert
        Assert.False(emailAddress.IsEmpty());
    }

    [Theory]
    [InlineData("old@example.com", "new@example.com")]
    [InlineData("user@test.net", "another.user@test.net")]
    public void Update_WithNewValidEmail_ReturnsSuccessResultWithUpdatedEmailAddress(string initialEmail, string newEmail)
    {
        // Arrange
        var emailAddress = EmailAddress.Create(initialEmail).Value;

        // Act
        var result = emailAddress.Update(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, result.Value.Email);
    }

    [Theory]
    [InlineData("valid@email.com", null)]
    [InlineData("valid@email.com", "")]
    [InlineData("valid@email.com", "invalid")]
    public void Update_WithNewInvalidEmail_ReturnsFailureResultWithValidationError(string initialEmail, string newEmail)
    {
        // Arrange
        var emailAddress = EmailAddress.Create(initialEmail).Value;

        // Act
        var result = emailAddress.Update(newEmail);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == nameof(EmailAddressException));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@subdomain.example.co.uk")]
    public void TryParse_ValidEmail_ReturnsSuccessResultWithEmailAddress(string email)
    {
        // Act
        var result = EmailAddress.TryParse(email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void TryParse_InvalidEmail_ReturnsFailureResultWithValidationError(string email)
    {
        // Act
        var result = EmailAddress.TryParse(email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Code == nameof(EmailAddressException));
    }

    [Fact]
    public void ToString_ReturnsEmailString()
    {
        // Arrange
        var email = "test@example.com";
        var emailAddress = EmailAddress.Create(email).Value;

        // Act
        var toStringResult = emailAddress.ToString();

        // Assert
        Assert.Equal(email, toStringResult);
    }
}