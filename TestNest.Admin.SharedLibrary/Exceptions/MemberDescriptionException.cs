namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class MemberDescriptionException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyDescription,
        LengthOutOfRange,
        NullEstablishmentMemberDescription
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.EmptyDescription, "Description cannot be empty." },
        { ErrorCode.LengthOutOfRange, "Description must be between 5 and 500 characters long." },
        { ErrorCode.NullEstablishmentMemberDescription, "Establishment member cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private MemberDescriptionException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private MemberDescriptionException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    public static MemberDescriptionException EmptyDescription() => new MemberDescriptionException(ErrorCode.EmptyDescription);

    public static MemberDescriptionException LengthOutOfRange() => new MemberDescriptionException(ErrorCode.LengthOutOfRange);

    public static MemberDescriptionException NullEstablishmentMemberDescription() => new MemberDescriptionException(ErrorCode.NullEstablishmentMemberDescription);
}