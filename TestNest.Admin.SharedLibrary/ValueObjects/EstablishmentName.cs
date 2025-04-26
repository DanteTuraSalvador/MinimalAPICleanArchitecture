using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class EstablishmentName : ValueObject
{
    private static readonly Regex ValidPattern = new(@"^[\p{L}0-9\s&,.'-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Lazy<EstablishmentName> _empty = new(() => new EstablishmentName());
    public string Name { get; }

    public bool IsEmpty() => this == Empty();

    public static EstablishmentName Empty() => _empty.Value;

    private EstablishmentName() => Name = string.Empty;

    private EstablishmentName(string value) => Name = value;

    public static Result<EstablishmentName> Create(string name)
    {
        var resultNull = Guard.AgainstNull(name, () => EstablishmentNameException.NullEstablishmentName());
        if (!resultNull.IsSuccess)
            return Result<EstablishmentName>.Failure(ErrorType.Validation, resultNull.Errors);

        var validationResult = ValidateEstablishmentName(name);
        return validationResult.IsSuccess
            ? Result<EstablishmentName>.Success(new EstablishmentName(name))
            : Result<EstablishmentName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<EstablishmentName> Update(string newName)
    {
        var resultNull = Guard.AgainstNull(newName, () => EstablishmentNameException.NullEstablishmentName());
        if (!resultNull.IsSuccess)
            return Result<EstablishmentName>.Failure(ErrorType.Validation, resultNull.Errors);

        return Create(newName);
    }

    private static Result ValidateEstablishmentName(string name)
    {
        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(name, () => EstablishmentNameException.EmptyName()),
            Guard.AgainstRange(name.Length, 3, 50, () => EstablishmentNameException.InvalidLength()),
            Guard.AgainstCondition(name != null && !ValidPattern.IsMatch(name), () => EstablishmentNameException.InvalidCharacters())
        );
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}