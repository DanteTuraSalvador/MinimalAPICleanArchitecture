using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Helpers;

public static class MinimalApiErrorHelper
{
    public static IResult HandleErrorResponse(HttpContext httpContext, ErrorType errorType, IEnumerable<Error>? errors = null)
    {
        List<Error> safeErrors = errors?.ToList() ?? new List<Error>();

        return errorType switch
        {
            ErrorType.Validation => Results.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "Validation failed.",
                Extensions = { ["errors"] = safeErrors }
            }),
            ErrorType.NotFound => Results.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "Resource not found.",
                Extensions = { ["errors"] = safeErrors }
            }),
            ErrorType.Conflict => Results.Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = "Resource conflict.",
                Extensions = { ["errors"] = safeErrors }
            }),
            _ => Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occurred.")
        };
    }
}
