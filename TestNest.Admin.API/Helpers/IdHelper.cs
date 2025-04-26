using System.Reflection;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.API.Helpers;

public static class IdHelper
{
    public static Result<TId> ValidateAndCreateId<TId>(string idString)
        where TId : StronglyTypedId<TId>
    {
        if (!Guid.TryParse(idString, out Guid parsedGuid))
        {
            var exception = StronglyTypedIdException.InvalidFormat();
            return Result<TId>.Failure(
                ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        try
        {
            if (typeof(TId).GetMethod("Create", BindingFlags.Public | BindingFlags.Static) is not MethodInfo createMethod)
            {
                return Result<TId>.Failure(
                    ErrorType.Internal,
                    new Error("CreateMethodNotFound", $"Create method not found for {typeof(TId).Name}."));
            }

            if (createMethod.Invoke(null, [parsedGuid]) is not Result<TId> result)
            {
                return Result<TId>.Failure(
                    ErrorType.Internal,
                    new Error("CreateMethodFailed", $"Create method failed for {typeof(TId).Name}."));
            }

            return result;
        }
        catch (Exception)
        {
            return Result<TId>.Failure(
                ErrorType.Internal,
                new Error("ReflectionError", $"Reflection error for {typeof(TId).Name}."));
        }
    }
}
