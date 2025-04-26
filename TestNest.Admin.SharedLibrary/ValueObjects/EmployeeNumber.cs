using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class EmployeeNumber : ValueObject
{
    private static readonly Regex EmployeeNumberRegex = new(@"^[A-Za-z0-9-]+$", RegexOptions.Compiled);
    private static readonly Lazy<EmployeeNumber> _empty = new(() => new EmployeeNumber());

    public bool IsEmpty() => this == Empty();

    public static EmployeeNumber Empty() => _empty.Value;

    public string EmployeeNo { get; }

    private EmployeeNumber() => EmployeeNo = string.Empty;

    private EmployeeNumber(string value) => EmployeeNo = value;

    public static Result<EmployeeNumber> Create(string employeeNumber)
    {
        var resultNull = Guard.AgainstNull(employeeNumber, () => EmployeeNumberException.NullEmployeeNumber());
        if (!resultNull.IsSuccess)
            return Result<EmployeeNumber>.Failure(ErrorType.Validation, resultNull.Errors);

        var result = Result.Combine(
            Guard.AgainstNullOrWhiteSpace(employeeNumber,
                () => EmployeeNumberException.EmptyEmployeeNumber()),
            Guard.AgainstCondition(!EmployeeNumberRegex.IsMatch(employeeNumber),
                () => EmployeeNumberException.InvalidEmployeeNumberFormat()),
            Guard.AgainstRange(employeeNumber.Length, 3, 10,
                () => EmployeeNumberException.LengthOutOfRangeEmployeeNumber())
        );
        return result.IsSuccess
            ? Result<EmployeeNumber>.Success(new EmployeeNumber(employeeNumber))
            : Result<EmployeeNumber>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EmployeeNumber> WithRoleName(string newEmployeeNumber)
        => Create(newEmployeeNumber);

    protected override IEnumerable<object?> GetAtomicValues() => new object?[] { EmployeeNo };

    public override string ToString() => EmployeeNo;
}