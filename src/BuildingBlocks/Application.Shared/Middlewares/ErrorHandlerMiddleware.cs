using Application.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Application.Shared.Middlewares
{
    /****
     * 
     * 
     * Take from the article CQRS ValidationPipeline With MediatR and FluentValidation
     * https://code-maze.com/cqrs-mediatr-fluentvalidation/
     * 
     * ****/
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = GetStatusCode(error);

                var responseModel = new
                {
                    Title = GetTitle(error),
                    Message = error.Message,
                    Succeeded = false,
                    Errors = GetErrors(error)
                };

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                DomainException => (int)HttpStatusCode.UnprocessableEntity,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

        private static string GetTitle(Exception exception) =>
            exception switch
            {
                ApiException appException => appException.Title,
                _ => "Internal Server Error"
            };

        private static IReadOnlyDictionary<string, string[]>? GetErrors(Exception exception)
        {
            IReadOnlyDictionary<string, string[]>? errors = null;
            if (exception is ValidationException validationException)
            {
                errors = validationException.ErrorsDictionary;
            }
            return errors;
        }
    }
}
