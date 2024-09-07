using AEBackendProject.Common;
using AEBackendProject.Common.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;

namespace AEBackendProject.Middleware
{
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
            catch (ShipAlreadyAssignedException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await HandleExceptionAsync(context, ex);
            }
            catch (ItemNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = Constant.GlobalExceptionMessage;

            _logger.LogError(exception, message);

            var result = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = message,
                Errors = new List<string> { exception.Message }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var json = JsonConvert.SerializeObject(result);

            return context.Response.WriteAsync(json);
        }
    }
}
