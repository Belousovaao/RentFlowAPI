using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Xml.Schema;
using RentFlow.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace RentFlow.Api.Middleware;

public sealed class ExceptionMiddleware
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
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
            await HandleExceptionAsync(context, ex);
        }

    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problem = ex switch
        {
            NotFoundException nf => CreateProblem(context, HttpStatusCode.NotFound, "Not Found", nf.Message),

            BusinessRuleViolationException br => CreateProblem(context, HttpStatusCode.BadRequest, "Business rule violation", br.Message),

            ValidationException ve => CreateValidationProblem(context, ve),

            _ => CreateProblem(context, HttpStatusCode.InternalServerError, "Server error", "An unexpected error occured")
        };

        context.Response.StatusCode = problem.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static ProblemDetails CreateProblem(HttpContext context, HttpStatusCode status, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private static HttpValidationProblemDetails CreateValidationProblem(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors.GroupBy(e => e.PropertyName)
                                     .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Instance = context.Request.Path
        };
    }
}
