using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;

    private static readonly string InternalServerErrorTitle = "Internal Server Error";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        using AsyncServiceScope scope = context.RequestServices.CreateAsyncScope();
        IErrorResponseService errorResponseService = scope.ServiceProvider.GetRequiredService<IErrorResponseService>();

        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        IActionResult actionResult = errorResponseService.CreateProblemDetails(
            (int)statusCode,
            InternalServerErrorTitle,
            "An unexpected error occurred.",
            new Dictionary<string, object> { { "type", $"{_configuration["ApiProblemDetails:BaseUrl"]}/internal-server-error" } });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        if (actionResult is ObjectResult objectResult && objectResult.Value is not null)
        {
            string json = JsonSerializer.Serialize(objectResult.Value);
            await context.Response.WriteAsync(json);
        }
    }
}
