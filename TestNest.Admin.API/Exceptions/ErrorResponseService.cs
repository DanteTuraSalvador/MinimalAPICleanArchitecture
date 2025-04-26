using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Exceptions;

public class ErrorResponseService : IErrorResponseService
{
    public IActionResult CreateBadRequestProblemDetails(string detail)
       => CreateProblemDetails(StatusCodes.Status400BadRequest, "Invalid Input", detail);

    public IActionResult CreateProblemDetails(int statusCode, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }

    public IActionResult CreateProblemDetails(int statusCode, string title, string detail, Dictionary<string, object> extensions)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
        };

        foreach (KeyValuePair<string, object> extension in extensions)
        {
            problemDetails.Extensions.Add(extension.Key, extension.Value);
        }

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}
