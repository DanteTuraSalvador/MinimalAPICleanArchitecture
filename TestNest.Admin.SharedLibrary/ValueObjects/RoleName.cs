using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed partial class RoleName : ValueObject
{
    [GeneratedRegex(@"^[A-Za-z0-9 &'_-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-PH")]
    private static partial Regex RoleNamePattern();

    private static readonly Regex DefaultPattern = RoleNamePattern();

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<RoleName> _empty = new(() => new RoleName());

    public static RoleName Empty() => _empty.Value;

    public string Name { get; }

    private RoleName() => Name = string.Empty;

    private RoleName(string value) => Name = value;

    public static Result<RoleName> Create(string roleName)
    {
        Result validationResult = ValidateRoleName(roleName);
        return validationResult.IsSuccess
            ? Result<RoleName>.Success(new RoleName(roleName))
            : Result<RoleName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<RoleName> WithRoleName(string newRoleName)
        => Create(newRoleName);

    public static Result<RoleName> TryParse(string roleName)
    {
        Result validationResult = ValidateRoleName(roleName);
        return validationResult.IsSuccess
            ? Result<RoleName>.Success(new RoleName(roleName))
            : Result<RoleName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    private static Result ValidateRoleName(string roleName)
    {
        Result resultNull = Guard.AgainstNull(roleName, static () => RoleNameException.NullRoleName());
        if (!resultNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultNull.Errors);
        }

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(roleName, static () => RoleNameException.EmptyRoleName()),
            Guard.AgainstCondition(!DefaultPattern.IsMatch(roleName), static () => RoleNameException.InvalidRoleNameCharacters()),
            Guard.AgainstRange(roleName.Length, 3, 100, static () => RoleNameException.InvalidRoleNameLength())
        );
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}
