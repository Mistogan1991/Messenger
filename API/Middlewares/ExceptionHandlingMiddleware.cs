using Messenger.Application.Common.Exceptions;
using Messenger.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace Messenger.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            Result response;
            HttpStatusCode statusCode;

            switch (ex)
            {
                case ValidationException ve:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new Result
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ValidationError,
                        Message = "Validation failed",
                        Errors = ve.Errors
                    };
                    break;

                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    response = new Result
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.NotFound,
                        Message = ex.Message
                    };
                    break;

                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    response = new Result
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.Unauthorized,
                        Message = ex.Message
                    };
                    break;

                case ForbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    response = new Result
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.Forbidden,
                        Message = ex.Message
                    };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    response = new Result
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ServerError,
                        Message = "Something went wrong"
                    };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
