using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    private static readonly Regex DomainRegex = new(@"^(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string Email { get; }
    private static readonly Lazy<EmailAddress> _empty = new(() => new EmailAddress());

    public static EmailAddress Empty() => _empty.Value;

    public bool IsEmpty() => this == Empty();

    private EmailAddress() => Email = string.Empty;

    private EmailAddress(string value) => Email = value;

    private static Result ValidateEmail(string email)
    {
        var validations = new List<Result>();
        validations.Add(Guard.AgainstNullOrWhiteSpace(email,
            () => EmailAddressException.InvalidFormat()));
        if (email != null)
        {
            validations.Add(Guard.AgainstCondition(email.Count(c => c == '@') != 1,
                () => EmailAddressException.InvalidFormat()));
            validations.Add(Guard.AgainstCondition(!EmailRegex.IsMatch(email),
                () => EmailAddressException.InvalidFormat()));
            if (EmailRegex.IsMatch(email))
            {
                var domain = email.Split('@')[1];
                validations.Add(Guard.AgainstCondition(!DomainRegex.IsMatch(domain),
                    () => EmailAddressException.InvalidFormat()));
            }
        }
        return Result.Combine(validations.ToArray());
    }

    public static Result<EmailAddress> Create(string email)
    {
        var validationResult = ValidateEmail(email);
        return validationResult.IsSuccess
            ? Result<EmailAddress>.Success(new EmailAddress(email!))
            : Result<EmailAddress>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<EmailAddress> Update(string newEmail) => Create(newEmail);

    public static Result<EmailAddress> TryParse(string email) => Create(email);

    protected override IEnumerable<object?> GetAtomicValues() => new object[] { Email };

    public override string ToString() => Email;
}