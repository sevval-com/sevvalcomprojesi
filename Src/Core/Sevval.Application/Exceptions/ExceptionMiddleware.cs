using Sevval.Application.Features.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;


namespace Sevval.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            if (exception.GetType() == typeof(ValidationException))
            {


                //return httpContext.Response.WriteAsync(new ExceptionModel
                //{
                //    Errors = ((ValidationException)exception).Errors?.Select(a => a.ErrorMessage),
                //    StatusCode = StatusCodes.Status400BadRequest

                //}.ToString());

                return httpContext.Response.WriteAsync(new ApiResponse
                {
                    Errors = ((ValidationException)exception).Errors?.Select(a => new ApiError { Message = a.ErrorMessage }).ToList(),
                    IsSuccessfull = false,
                    Message = string.Join(",",((ValidationException)exception).Errors.ToList()),
                    StatusCode = statusCode,


                }.ToString());
            }

            List<string> Errors = new()
            {
                $"Hata Mesajı :{exception.Message}"
            };

            //return httpContext.Response.WriteAsync(new ExceptionModel
            //{
            //    Errors = Errors,
            //    StatusCode = statusCode
            //}.ToString());

            return httpContext.Response.WriteAsync(new ApiResponse
             {
                 Errors = Errors?.Select(a => new ApiError { Message = a }).ToList(),
                 IsSuccessfull = false,
                 Message = exception.Message,
                 Code = statusCode,
                 StatusCode = statusCode,

             }.ToString());

            
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                TooManyRequestsException => StatusCodes.Status429TooManyRequests,
                _ => StatusCodes.Status500InternalServerError
            };
    }

}
