using System.Net;
using System.Text.Json;
using ChessTournamentCalendarBackend.API.DTOs; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TournamentApp.API.Middlewares; 

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
            
            _logger.LogError(ex, "🔥 \n A system error occurred! Request Path: {Path}", context.Request.Path);

            
            await HandleExceptionAsync(context);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var response = ApiResponse<object>.ErrorResponse("Server error");
        
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        return context.Response.WriteAsync(json);
    }
}