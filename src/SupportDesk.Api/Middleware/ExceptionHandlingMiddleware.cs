using System.Net;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title, code, detail) = exception switch
        {
            DomainException domain when domain.Code is ErrorCodes.TicketNotFound or ErrorCodes.AgentNotFound
                => (HttpStatusCode.NotFound, "Not found", domain.Code, domain.Message),
            DomainException domain
                => (HttpStatusCode.UnprocessableEntity, "Business rule violated", domain.Code, domain.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", "UNEXPECTED_ERROR",
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
        };

        if ((int)status >= 500)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogInformation(exception, "Request rejected: {Code}", code);
        }

        var problem = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{(int)status}",
            Instance = context.Request.Path
        };
        problem.Extensions["code"] = code;

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
