using Bored_But_Broke_back_end.ExternalApis;
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
            var problemDetails = new ProblemDetails
            {
                Status = exception switch
                {
                    BadHttpRequestException => (int?)((BadHttpRequestException)exception).StatusCode,
                    HttpRequestException => StatusCodes.Status502BadGateway,
                    TaskCanceledException => StatusCodes.Status504GatewayTimeout,
                    ExternalApiException => (int?)((ExternalApiException)exception).StatusCode,
                    _ => StatusCodes.Status500InternalServerError
                },
                Title = exception switch
                {
                    BadHttpRequestException => "Bad Request",
                    HttpRequestException => "Bad Gateway",
                    TaskCanceledException => "Gateway Timeout",
                    ExternalApiException => ((ExternalApiException)exception).ErrorTitle,
                    _ => "Internal Server Error"
                },
                Detail = exception switch
                {
                    BadHttpRequestException => exception.Message,
                    HttpRequestException => exception.Message,
                    TaskCanceledException => exception.Message,
                    ExternalApiException => ((ExternalApiException)exception).ErrorDetail,
                    _ => "Please try again later."
                }
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

            await httpContext.Response.WriteAsJsonAsync(problemDetails, token);

            return true;
        }
    }
}
