using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrderService.Grpc.Gateway.ExceptionHandlers;

internal sealed class GrpcExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GrpcExceptionHandler> _logger;

    public GrpcExceptionHandler(ILogger<GrpcExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not RpcException rpcException)
        {
            return false;
        }

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails();

        switch (rpcException.Status.StatusCode)
        {
            case StatusCode.NotFound:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not found";
                break;
            case StatusCode.AlreadyExists:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                break;
            case StatusCode.InvalidArgument:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                break;
            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal server error";
                break;
        }

        problemDetails.Detail = rpcException.Message;

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}