namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class MemberTagException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyTag,
        InvalidFormat,
        LengthOutOfRange,
        NullMemberTag,
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.EmptyTag, "Tag cannot be empty." },
        { ErrorCode.InvalidFormat, "Tag can only contain letters, numbers, and spaces." },
        { ErrorCode.LengthOutOfRange, "Tag must be between 3 and 100 characters long." },
        { ErrorCode.NullMemberTag, "Tag cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private MemberTagException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private MemberTagException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    public static MemberTagException EmptyTag() => new MemberTagException(ErrorCode.EmptyTag);

    public static MemberTagException InvalidFormat() => new MemberTagException(ErrorCode.InvalidFormat);

    public static MemberTagException LengthOutOfRange() => new MemberTagException(ErrorCode.LengthOutOfRange);

    public static MemberTagException NullMemberTag() => new MemberTagException(ErrorCode.NullMemberTag);
}