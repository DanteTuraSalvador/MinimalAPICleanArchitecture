using Microsoft.AspNetCore.Mvc;

namespace TestNest.Admin.SharedLibrary.Exceptions.Common;

public interface IErrorResponseService
{
    IActionResult CreateBadRequestProblemDetails(string detail);

    IActionResult CreateProblemDetails(int statusCode, string title, string detail);

    IActionResult CreateProblemDetails(int statusCode, string title, string detail, Dictionary<string, object> extensions);
}