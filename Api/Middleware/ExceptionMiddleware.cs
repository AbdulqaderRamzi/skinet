using System.Text.Json;
using Api.Errors;

namespace Api.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception e)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        var response = env.IsDevelopment() 
            ? new ApiErrorResponse(httpContext.Response.StatusCode, e.Message, e.StackTrace)
            : new ApiErrorResponse(httpContext.Response.StatusCode, e.Message, "Internal Server Error");
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await httpContext.Response.WriteAsync(json);
    }
}