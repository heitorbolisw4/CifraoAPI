using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.EndPoints.ExceptionHandlers;



public sealed class ExternalApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if( exception is not HttpRequestException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status502BadGateway,
            Title = "Bad Gateway"
            
        };
        httpContext.Response.StatusCode = StatusCodes.Status502BadGateway;
    
        await httpContext.Response.WriteAsJsonAsync(problemDetails, (JsonSerializerOptions?)null, "application/problem+json",cancellationToken);
        return true;
    
    }
}