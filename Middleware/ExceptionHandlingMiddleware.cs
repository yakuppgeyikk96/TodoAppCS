
using System.Net;
using System.Text.Json;
using FirstWebApi.Exceptions;
using FirstWebApi.Models;

namespace FirstWebApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
  private readonly RequestDelegate _next = next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

  public async Task InvokeAsync(HttpContext context)
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

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    var response = context.Response;

    var errorResponse = new ApiResponse<object>
    {
      Success = false,
      Message = exception.Message
    };

    switch (exception)
    {
      case NotFoundException:
        response.StatusCode = (int)HttpStatusCode.NotFound;
        errorResponse.Status = (int)HttpStatusCode.NotFound;
        break;

      case BadRequestException:
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        errorResponse.Status = (int)HttpStatusCode.BadRequest;
        break;

      case UnauthorizedException:
        response.StatusCode = (int)HttpStatusCode.Unauthorized;
        errorResponse.Status = (int)HttpStatusCode.Unauthorized;
        break;

      case KeyNotFoundException:
        response.StatusCode = (int)HttpStatusCode.NotFound;
        errorResponse.Status = (int)HttpStatusCode.NotFound;
        errorResponse.Message = "The requested resource was not found.";
        break;

      default:
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        errorResponse.Status = (int)HttpStatusCode.InternalServerError;

        if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
          errorResponse.Message = $"An error occurred: {exception.Message}\nStack Trace: {exception.StackTrace}";
        }
        else
        {
          errorResponse.Message = "An internal server error occurred. Please try again later.";
        }

        _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
        break;
    }

    var options = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    var jsonResponse = JsonSerializer.Serialize(errorResponse, options);
    await response.WriteAsync(jsonResponse);
  }
}
