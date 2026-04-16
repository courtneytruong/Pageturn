using Microsoft.AspNetCore.Mvc;
using Pageturn.Core.Exceptions;
using System.Net;

namespace Pageturn.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Detail = exception.Message;
                break;

            case UnauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = exception.Message;
                break;

            case ConflictException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                problemDetails.Detail = exception.Message;
                break;

            case ValidationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message;
                break;

            default:
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
