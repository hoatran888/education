using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Common.Exceptions;

namespace SchoolSystem.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate                   _next;
    private readonly ILogger<ExceptionMiddleware>      _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            NotFoundException  => (StatusCodes.Status404NotFound,                 "Not Found"),
            ForbiddenException => (StatusCodes.Status403Forbidden,                "Forbidden"),
            ConflictException  => (StatusCodes.Status409Conflict,                 "Conflict"),
            ValidationException => (StatusCodes.Status422UnprocessableEntity,     "Validation Error"),
            _                  => (StatusCodes.Status500InternalServerError,      "Internal Server Error")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode  = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = exception.Message
        };

        if (exception is ValidationException ve)
        {
            problem.Extensions["errors"] = ve.Errors
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray());
        }

        await context.Response.WriteAsJsonAsync(problem);
    }
}
