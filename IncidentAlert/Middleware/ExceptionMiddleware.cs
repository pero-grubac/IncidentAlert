using IncidentAlert.Exceptions;
using Serilog;
using System.Net;

namespace IncidentAlert.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            ErrorDetails details = ex switch
            {
                EntityDoesNotExistException custom => new ErrorDetails
                {
                    Code = (int)HttpStatusCode.NotFound, // 404 Not Found
                    Message = custom.GetBaseMessage(),
                },
                EntityCannotBeDeletedException custom => new ErrorDetails
                {
                    Code = (int)HttpStatusCode.Conflict,  // 409 Conflict
                    Message = custom.GetBaseMessage(),
                },
                EntityCanNotBeCreatedException custom => new ErrorDetails
                {
                    Code = (int)HttpStatusCode.BadRequest, // 404 Not Found
                    Message = custom.GetBaseMessage(),
                },
                ArgumentException => new ErrorDetails
                {
                    Code = (int)HttpStatusCode.BadRequest,  // 400 Bad Request
                    Message = ex.Message
                },
                _ => new ErrorDetails
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                }
            };

            Log.Error("Error details: {@error}", details.ToString());
            if (ex.InnerException != null)
            {
                Log.Error("Inner exception: {@innerException}", ex.InnerException);
            }

            context.Response.StatusCode = details.Code;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(details.ToString());
        }

    }
}
