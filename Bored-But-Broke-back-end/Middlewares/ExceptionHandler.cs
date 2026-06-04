using Bored_But_Broke_back_end.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bored_But_Broke_back_end.Middlewares
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        public ExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
        {
            var (statusCode, title, detail) = exception switch
            {
                BadHttpRequestException e => (e.StatusCode, "Bad Request", e.Message),
                RegistrationFailedException e => (StatusCodes.Status400BadRequest, "Bad Request", e.Message),
                FavouriteNotFoundException e => (StatusCodes.Status400BadRequest, "Bad Request", e.Message),
                LoginUnsuccessfulException e => (StatusCodes.Status401Unauthorized, "Unauthorized", e.Message),
                EmailAlreadyInUseException e => (StatusCodes.Status409Conflict, "Conflict", e.Message),
                PlaceAlreadyFavouritedException e => (StatusCodes.Status409Conflict, "Conflict", e.Message),
                UserLockedOutException e => (StatusCodes.Status429TooManyRequests, "Too Many Requests", e.Message),
                HttpRequestException e => (StatusCodes.Status502BadGateway, "Bad Gateway", e.Message),
                TaskCanceledException e => (StatusCodes.Status504GatewayTimeout, "Gateway Timeout", e.Message),
                ExternalApiException e => ((int?)e.StatusCode, e.ErrorTitle, e.ErrorDetail),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Please try again later.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}
