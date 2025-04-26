using TestNest.Admin.SharedLibrary.Common.Guards;

namespace TestNest.Admin.SharedLibrary.ValueObjects.Enums;

public sealed class EstablishmentStatus
{
    public static readonly EstablishmentStatus None = new(-1, "None");
    public static readonly EstablishmentStatus Pending = new(0, "Pending");
    public static readonly EstablishmentStatus Approval = new(1, "Approval");
    public static readonly EstablishmentStatus Rejected = new(2, "Rejected");
    public static readonly EstablishmentStatus Active = new(3, "Active");
    public static readonly EstablishmentStatus InActive = new(4, "InActive");
    public static readonly EstablishmentStatus Suspended = new(5, "Suspended");

    private static readonly Dictionary<EstablishmentStatus, List<EstablishmentStatus>> _transitions = new()
    {
        [None] = new() { Pending },
        [Pending] = new() { Approval, Rejected },
        [Approval] = new() { Active },
        [Active] = new() { InActive, Suspended },
        [Suspended] = new() { Active },
        [InActive] = new() { Active },
        [Rejected] = new() { Pending }
    };

    public int Id { get; }
    public string Name { get; }

    private EstablishmentStatus(int id, string name) => (Id, Name) = (id, name);

    public static Result ValidateTransition(EstablishmentStatus current, EstablishmentStatus next)
    {
        var nullCheck = Result.Combine(
            Guard.AgainstNull(current, () => EstablishmentStatusException.NullStatus()),
            Guard.AgainstNull(next, () => EstablishmentStatusException.NullStatus())
        );

        if (!nullCheck.IsSuccess)
        {
            return Result.Failure(
                ErrorType.Validation,
                nullCheck.Errors.Select(e => new Error(
                    nameof(EstablishmentStatusException.ErrorCode.NullStatus),
                    e.Message
                ))
            );
        }

        Result transitionCheck = Guard.AgainstCondition(
            !IsValidTransition(current, next),
            () => EstablishmentStatusException.InvalidStatusTransition(current, next)
        );

        return transitionCheck.IsSuccess
            ? Result.Success()
            : Result.Failure(
                ErrorType.Validation,
                new Error(
                    nameof(EstablishmentStatusException.ErrorCode.InvalidStatusTransition),
                    transitionCheck.Errors.First().Message
                )
            );
    }

    //private static bool IsValidTransition(EstablishmentStatus current, EstablishmentStatus next)
    //    => _transitions.TryGetValue(current, out var allowed) && allowed.Contains(next);

    private static bool IsValidTransition(EstablishmentStatus current, EstablishmentStatus next)
    => _transitions.TryGetValue(current, out List<EstablishmentStatus> allowed) && allowed.Contains(next);

    public static EstablishmentStatus FromIdOrDefault(int id) => All.FirstOrDefault(status => status.Id == id) ?? None;

    public static EstablishmentStatus FromNameOrDefault(string name)
        => All.FirstOrDefault(status => status.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? None;

    public static Result<EstablishmentStatus> FromId(int id)
    {
        EstablishmentStatus status = All.FirstOrDefault(s => s.Id == id);
        return status != null
            ? Result<EstablishmentStatus>.Success(status)
            : Result<EstablishmentStatus>.Failure(
                ErrorType.NotFound,
                new Error(
                    nameof(EstablishmentStatusException.ErrorCode.UnknownStatusId),
                    EstablishmentStatusException.UnknownStatusId(id).Message
                )
            );
    }

    public static Result<EstablishmentStatus> FromName(string name)
    {
        EstablishmentStatus status = All.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return status != null
            ? Result<EstablishmentStatus>.Success(status)
            : Result<EstablishmentStatus>.Failure(
                ErrorType.NotFound,
                new Error(
                    ((int)EstablishmentStatusException.ErrorCode.UnknownStatusName).ToString(),
                    EstablishmentStatusException.UnknownStatusName(name).Message
                )
            );
    }

    public static IReadOnlyList<EstablishmentStatus> All => new[]
    {
        None, Pending, Approval, Rejected, Active, InActive, Suspended
    };

    public override bool Equals(object obj)
        => obj is EstablishmentStatus status && Id == status.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EstablishmentStatus left, EstablishmentStatus right)
        => EqualityComparer<EstablishmentStatus>.Default.Equals(left, right);

    public static bool operator !=(EstablishmentStatus left, EstablishmentStatus right)
        => !(left == right);
}
