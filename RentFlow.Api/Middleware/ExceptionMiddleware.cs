using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Xml.Schema;
using RentFlow.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";
            
        ProblemDetails problem = ex switch
        {
            NotFoundException nf => CreateProblem(context, 404, "Not Found", nf.Message),

            BusinessRuleViolationException br => CreateProblem(context, 400, "Business rule violation", br.Message),

            ValidationException ve => CreateValidationProblem(context, ve),

            _ => CreateProblem(context, 500, ex.GetType().Name, ex.ToString())
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static ProblemDetails CreateProblem(HttpContext context, int status, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = status,
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
            Status = 400,
            Title = "Validation failed",
            Instance = context.Request.Path
        };
    }
}
