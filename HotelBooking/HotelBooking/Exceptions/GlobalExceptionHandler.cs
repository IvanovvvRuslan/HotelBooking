using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            NotFoundException notFoundException => HandleNotFoundException(notFoundException),
            _ => HandleUnknownException(exception)
        };
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private static (int statusCode, object response) HandleNotFoundException(NotFoundException exception)
    {
        return (StatusCodes.Status404NotFound, new
        {
            Status = "Error",
            Message = exception.Message,
            Details = (object?)null
        });
    }

    private static (int statusCode, object response) HandleUnknownException(Exception exception)
    {
        return (StatusCodes.Status500InternalServerError, new
        {
            Status = "Error",
            Message = exception.Message,
            Details = (object?)null
        });
    }
}