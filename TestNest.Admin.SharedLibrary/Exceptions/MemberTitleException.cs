namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class MemberTitleException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyTitle,
        InvalidFormat,
        LengthOutOfRange,
        NullMemberTitle
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.EmptyTitle, "Title cannot be empty." },
        { ErrorCode.InvalidFormat, "Title can only contain letters and spaces." },
        { ErrorCode.LengthOutOfRange, "Title must be between 2 and 100 characters long." },
        { ErrorCode.NullMemberTitle, "Title cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private MemberTitleException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private MemberTitleException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    public static MemberTitleException EmptyTitle() => new MemberTitleException(ErrorCode.EmptyTitle);

    public static MemberTitleException InvalidFormat() => new MemberTitleException(ErrorCode.InvalidFormat);

    public static MemberTitleException LengthOutOfRange() => new MemberTitleException(ErrorCode.LengthOutOfRange);

    public static MemberTitleException NullMemberTitle() => new MemberTitleException(ErrorCode.NullMemberTitle);
}