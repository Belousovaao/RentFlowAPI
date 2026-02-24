using System;
using MediatR;

namespace RentFlow.Api.Middleware;

public sealed class RequestLoggingMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<RequestLoggingMiddleware<TRequest, TResponse>> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requesrName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} {@Request}", requesrName, request);

        var response = await next();
        _logger.LogInformation("Handled {RequestName}", requesrName);

        return response;
    }
}
