namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EstablishmentStatusException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        InvalidStatusTransition,
        UnknownStatusId,
        UnknownStatusName,
        NullStatus
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.InvalidStatusTransition, "Invalid transition between establishment statuses." },
        { ErrorCode.UnknownStatusId, "No establishment status found for the specified ID." },
        { ErrorCode.UnknownStatusName, "No establishment status found for the specified name." },
        { ErrorCode.NullStatus, "Establishment status cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    private EstablishmentStatusException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    // Static factory methods
    public static EstablishmentStatusException InvalidStatusTransition(EstablishmentStatus current, EstablishmentStatus next)
        => new(ErrorCode.InvalidStatusTransition);

    public static EstablishmentStatusException UnknownStatusId(int id)
        => new(ErrorCode.UnknownStatusId);

    public static EstablishmentStatusException UnknownStatusName(string name)
        => new(ErrorCode.UnknownStatusName);

    public static EstablishmentStatusException NullStatus()
        => new(ErrorCode.NullStatus);
}