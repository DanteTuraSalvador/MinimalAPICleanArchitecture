namespace TestNest.Admin.SharedLibrary.Exceptions.Common;

public interface IHasErrorCode
{
    string Code { get; }
    string Message { get; }
}