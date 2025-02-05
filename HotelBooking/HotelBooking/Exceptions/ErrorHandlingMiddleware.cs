using System.Net;

namespace HotelBooking.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        int statusCode = ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = statusCode;
        
        var response = new {error = ex.Message, statusCode = statusCode};
        
        return context.Response.WriteAsJsonAsync(response);
    }
}