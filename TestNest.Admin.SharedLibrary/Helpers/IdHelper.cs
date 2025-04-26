using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.Helpers;

public static class IdHelper
{
    public const string InvalidGuidFormatErrorCode = "InvalidGuidFormat";
    public const string CreateMethodNotFoundErrorCode = "CreateMethodNotFound";
    public const string CreateMethodFailedErrorCode = "CreateMethodFailed";
    public const string ReflectionErrorErrorCode = "ReflectionError";

    public static Result<TId> ValidateAndCreateId<TId>(string idString)
        where TId : StronglyTypedId<TId>
    {
        if (!Guid.TryParse(idString, out Guid parsedGuid))
        {
            return Result<TId>.Failure(
                ErrorType.Validation,
                new Error(InvalidGuidFormatErrorCode, IdValidationException.InvalidGuidFormat().Message));
        }

        try
        {
            var createMethod = typeof(TId).GetMethod("Create", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (createMethod == null)
            {
                return Result<TId>.Failure(
                    ErrorType.Internal,
                    new Error(CreateMethodNotFoundErrorCode, $"Create method not found for {typeof(TId).Name}."));
            }

            var result = createMethod.Invoke(null, new object[] { parsedGuid }) as Result<TId>;

            if (result == null)
            {
                return Result<TId>.Failure(
                    ErrorType.Internal,
                    new Error(CreateMethodFailedErrorCode, $"Create method failed for {typeof(TId).Name}."));
            }

            return result;
        }
        catch (IdValidationException ex)
        {
            return Result<TId>.Failure(
                ErrorType.Validation,
                new Error(ex.ErrorCode, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<TId>.Failure(
                ErrorType.Internal,
                new Error(ReflectionErrorErrorCode, $"Reflection error for {typeof(TId).Name}. {ex.Message}"));
        }
    }
}