using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class PersonName : ValueObject
{
    private static readonly Regex NamePattern = new(@"^\p{L}+(?:[-'\s]\p{L}+)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Lazy<PersonName> _empty = new(() => new PersonName());
    public string FirstName { get; }
    public string? MiddleName { get; }
    public string LastName { get; }

    public static PersonName Empty() => _empty.Value;

    private PersonName()
    {
        FirstName = string.Empty;
        MiddleName = string.Empty;
        LastName = string.Empty;
    }

    private PersonName(string firstName, string? middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }

    public static Result<PersonName> Create(string firstName, string? middleName, string lastName)
    {
        var validationResult = ValidatePersonName(firstName, middleName, lastName);
        return validationResult.IsSuccess
            ? Result<PersonName>.Success(new PersonName(firstName, middleName, lastName))
            : Result<PersonName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<PersonName> Update(string firstName, string? middleName, string lastName)
        => Create(firstName, middleName, lastName);

    private static Result ValidatePersonName(string firstName, string? middleName, string lastName)
    {
        var resultFirstnNameNull = Guard.AgainstNull(firstName, () => PersonNameException.NullPersonName());
        if (!resultFirstnNameNull.IsSuccess)
            return Result.Failure(ErrorType.Validation, resultFirstnNameNull.Errors);

        var resultLastName = Guard.AgainstNull(lastName, () => PersonNameException.NullPersonName());
        if (!resultLastName.IsSuccess)
            return Result.Failure(ErrorType.Validation, resultLastName.Errors);

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(firstName, () => PersonNameException.EmptyFirstName()),
            Guard.AgainstNullOrWhiteSpace(lastName, () => PersonNameException.EmptyLastName()),
            Guard.AgainstCondition(!NamePattern.IsMatch(firstName) || !NamePattern.IsMatch(lastName) || (middleName is not null && !NamePattern.IsMatch(middleName)), () => PersonNameException.InvalidCharacters()),
            Guard.AgainstCondition(firstName.Length < 2 || firstName.Length > 100 || lastName.Length < 2 || lastName.Length > 100 || (middleName is not null && (middleName.Length < 2 || middleName.Length > 100)), () => PersonNameException.InvalidLength())
        );
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        if (MiddleName is not null)
            yield return MiddleName;
        yield return LastName;
    }

    public bool IsEmpty() => this == Empty();

    public override string ToString() => GetFullName();

    public string GetFullName()
    {
        return string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";
    }
}
