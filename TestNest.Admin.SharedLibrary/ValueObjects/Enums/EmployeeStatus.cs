using TestNest.Admin.SharedLibrary.Common.Guards;

namespace TestNest.Admin.SharedLibrary.ValueObjects.Enums;

public sealed class EmployeeStatus
{
    public static readonly EmployeeStatus None = new(-1, "None");
    public static readonly EmployeeStatus Active = new(0, "Active");
    public static readonly EmployeeStatus Inactive = new(1, "Inactive");
    public static readonly EmployeeStatus Suspended = new(2, "Suspended");

    private static readonly Dictionary<EmployeeStatus, List<EmployeeStatus>> _transitions = new()
    {
        [None] = new() { Active },
        [Active] = new() { Inactive, Suspended },
        [Inactive] = new() { Active },
        [Suspended] = new() { Inactive }
    };

    public int Id { get; }
    public string Name { get; }

    private EmployeeStatus(int id, string name) => (Id, Name) = (id, name);

    public static Result ValidateTransition(EmployeeStatus current, EmployeeStatus next)
    {
        var nullCheck = Result.Combine(
            Guard.AgainstNull(current, () => EmployeeStatusException.NullStatus()),
            Guard.AgainstNull(next, () => EmployeeStatusException.NullStatus())
        );

        if (!nullCheck.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, nullCheck.Errors);
        }

        Result transitionCheck = Guard.AgainstCondition(
            !IsValidTransition(current, next),
            () => EmployeeStatusException.InvalidStatusTransition(current, next));

        return transitionCheck.IsSuccess
            ? Result.Success()
            : Result.Failure(ErrorType.Validation,
                new Error(nameof(EmployeeStatusException.ErrorCode.InvalidStatusTransition),
                    transitionCheck.Errors.First().Message));
    }

    public static EmployeeStatus FromIdOrDefault(int id) => All.FirstOrDefault(status => status.Id == id) ?? None;

    public static EmployeeStatus FromNameOrDefault(string name)
        => All.FirstOrDefault(status => status.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? None;

    //private static bool IsValidTransition(EmployeeStatus current, EmployeeStatus next)
    //    => _transitions.TryGetValue(current, out var allowed) && allowed.Contains(next);

    private static bool IsValidTransition(EmployeeStatus current, EmployeeStatus next)
    => _transitions.TryGetValue(current, out List<EmployeeStatus> allowed) && allowed.Contains(next);

    public static Result<EmployeeStatus> FromId(int id)
    {
        EmployeeStatus status = All.FirstOrDefault(s => s.Id == id);
        return status != null
            ? Result<EmployeeStatus>.Success(status)
            : Result<EmployeeStatus>.Failure(ErrorType.NotFound,
                new Error(nameof(EmployeeStatusException.ErrorCode.UnknownStatusId),
                    EmployeeStatusException.UnknownStatusId(id).Message));
    }

    public static Result<EmployeeStatus> FromName(string name)
    {
        EmployeeStatus status = All.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return status != null
            ? Result<EmployeeStatus>.Success(status)
            : Result<EmployeeStatus>.Failure(ErrorType.NotFound,
                new Error(((int)EmployeeStatusException.ErrorCode.UnknownStatusName).ToString(),
                    EmployeeStatusException.UnknownStatusName(name).Message));
    }

    public static IReadOnlyList<EmployeeStatus> All
        => new[] { None, Active, Inactive, Suspended };

    public override bool Equals(object obj)
        => obj is EmployeeStatus status && Id == status.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EmployeeStatus left, EmployeeStatus right)
        => EqualityComparer<EmployeeStatus>.Default.Equals(left, right);

    public static bool operator !=(EmployeeStatus left, EmployeeStatus right)
        => !(left == right);
}
