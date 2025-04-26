using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class RoleNameTests
{
    public static IEnumerable<object[]> ValidRoleNames => new List<object[]>
    {
        new object[] { "Admin" },
        new object[] { "Super Admin" },
        new object[] { "Content Editor" },
        new object[] { "User-Manager" },
        new object[] { "Role_Admin" },
        new object[] { "Permission's Editor" },
        new object[] { "Test & Dev" },
        new object[] { new string('A', 3) },
        new object[] { new string('Z', 100) }
    };

    public static IEnumerable<object[]> InvalidRoleNameData => new List<object[]>
    {
        new object[] { null, "Role name cannot be null." },
        new object[] { "", "Role name cannot be empty." },
        new object[] { " ", "Role name cannot be empty." },
        new object[] { "Inv@lid", "Role name contains invalid characters." },
        new object[] { "!Role", "Role name contains invalid characters." },
        new object[] { "Role#", "Role name contains invalid characters." },
        new object[] { "ab", "Role name must be between 3 and 100 characters." },
        new object[] { new string('A', 101), "Role name must be between 3 and 100 characters." }
    };

    [Theory]
    [MemberData(nameof(ValidRoleNames))]
    public void Create_ValidRoleName_ReturnsSuccessResult(string roleName)
        => Assert.True(RoleName.Create(roleName).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidRoleNameData))]
    public void Create_InvalidRoleName_ReturnsFailureResultWithValidationError(string roleName, string expectedErrorMessage)
    {
        var result = RoleName.Create(roleName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyRoleNameInstance()
        => Assert.True(RoleName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
        => Assert.True(RoleName.Empty().IsEmpty());

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
        => Assert.False(RoleName.Create("Admin").Value.IsEmpty());

    [Theory]
    [MemberData(nameof(ValidRoleNames))]
    public void WithRoleName_WithNewValidRoleName_ReturnsSuccessResultWithUpdatedRoleName(string newRoleName)
        => Assert.True(RoleName.Create("OldRole").Value.WithRoleName(newRoleName).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidRoleNameData))]
    public void WithRoleName_WithNewInvalidRoleName_ReturnsFailureResultWithValidationError(string newRoleName, string expectedErrorMessage)
    {
        var initialRoleName = RoleName.Create("OldRole").Value;
        var result = initialRoleName.WithRoleName(newRoleName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Theory]
    [MemberData(nameof(ValidRoleNames))]
    public void TryParse_ValidRoleName_ReturnsSuccessResult(string roleName)
        => Assert.True(RoleName.TryParse(roleName).IsSuccess);

    [Theory]
    [MemberData(nameof(InvalidRoleNameData))]
    public void TryParse_InvalidRoleName_ReturnsFailureResultWithValidationError(string roleName, string expectedErrorMessage)
    {
        var result = RoleName.TryParse(roleName);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, error => error.Message == expectedErrorMessage);
    }

    [Fact]
    public void ToString_ReturnsRoleNameString()
        => Assert.Equal("Admin", RoleName.Create("Admin").Value.ToString());
}