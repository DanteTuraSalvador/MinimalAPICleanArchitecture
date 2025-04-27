using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class PhoneNumberTests
{
    public static IEnumerable<object[]> ValidPhoneNumbers => new List<object[]>
    {
        new object[] { "+1234567890" },
        new object[] { "1234567" },
        new object[] { "987654321012345" },
        new object[] { "+639123456789" }
    };

    public static IEnumerable<object[]> InvalidPhoneNumberData => new List<object[]>
    {
        new object[] { null, "Phone number cannot be null." },
        new object[] { "", "Phone number cannot be empty." },
        new object[] { " ", "Phone number cannot be empty." },
        new object[] { "abc", "Invalid phone number format. Use E.164 format (e.g., +1234567890)." },
        new object[] { "+abc", "Invalid phone number format. Use E.164 format (e.g., +1234567890)." },
        new object[] { "123456", "Invalid phone number format. Use E.164 format (e.g., +1234567890)." },
        new object[] { "1234567890123456", "Invalid phone number format. Use E.164 format (e.g., +1234567890)." }
    };

    [Theory]
    [MemberData(nameof(ValidPhoneNumbers))]
    public void Create_ValidPhoneNumber_ReturnsSuccessResult(string phoneNumber)
        => Assert.True(PhoneNumber.Create(phoneNumber).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidPhoneNumberData))]
    public void Create_InvalidPhoneNumber_ReturnsFailureResultWithValidationError(string phoneNumber, string expectedErrorMessage)
    {
        var result = PhoneNumber.Create(phoneNumber);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyPhoneNumberInstance()
        => Assert.True(PhoneNumber.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(PhoneNumber.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(PhoneNumber.Create("+1234567890").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidPhoneNumbers))]
    public void Update_WithNewValidPhoneNumber_ReturnsSuccessResultWithUpdatedPhoneNumber(string newPhoneNumber)
    {
        var initialNumber = PhoneNumber.Create("1112223333").Value;
        Assert.True(PhoneNumber.Update(newPhoneNumber).IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidPhoneNumberData))]
    public void Update_WithNewInvalidPhoneNumber_ReturnsFailureResultWithValidationError(string newPhoneNumber, string expectedErrorMessage)
    {
        var initialNumber = PhoneNumber.Create("1112223333").Value;
        var result = PhoneNumber.Update(newPhoneNumber);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void GetFormatted_WithPlus_ReturnsSameNumber()
        => Assert.Equal("+1234567890", PhoneNumber.Create("+1234567890").Value.GetFormatted());

    [Fact]
    public void GetFormatted_WithoutPlus_AddsPlus()
        => Assert.Equal("+1234567", PhoneNumber.Create("1234567").Value.GetFormatted());

    [Fact]
    public void ToString_ReturnsFormattedNumber()
        => Assert.Equal("+987654321012345", PhoneNumber.Create("987654321012345").Value.ToString());
}
