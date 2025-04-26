using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(@"^\+?\d{7,15}$", RegexOptions.Compiled);
    private static readonly Lazy<PhoneNumber> _empty = new(() => new PhoneNumber());
    public string PhoneNo { get; }

    public bool IsEmpty() => this == Empty();

    public static PhoneNumber Empty() => _empty.Value;

    private PhoneNumber() => PhoneNo = string.Empty;

    private PhoneNumber(string value) => PhoneNo = value;

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        Result validationResult = ValidatePhoneNumber(phoneNumber);
        return validationResult.IsSuccess
            ? Result<PhoneNumber>.Success(new PhoneNumber(phoneNumber))
            : Result<PhoneNumber>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public static Result<PhoneNumber> Update(string newPhoneNumber)
        => Create(newPhoneNumber);

    private static Result ValidatePhoneNumber(string phoneNumber)
    {
        Result resultNull = Guard.AgainstNull(phoneNumber, static () => PhoneNumberException.NullPhoneNumber());
        if (!resultNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultNull.Errors);
        }

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(phoneNumber, static () => PhoneNumberException.EmptyPhoneNumber()),
            Guard.AgainstCondition(!PhoneRegex.IsMatch(phoneNumber), static () => PhoneNumberException.InvalidFormat())
        );
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return PhoneNo;
    }

    public string GetFormatted() => PhoneNo.StartsWith("+") ? PhoneNo : $"+{PhoneNo}";

    public override string ToString() => GetFormatted();
}
