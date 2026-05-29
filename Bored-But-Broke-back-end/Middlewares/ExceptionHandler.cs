using Bored_But_Broke_back_end.ExternalApis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;

namespace Bored_But_Broke_back_end.Middlewares
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
        {
            var problemDetails = new ProblemDetails
            {
                Status = exception switch
                {
                    HttpRequestException => StatusCodes.Status502BadGateway,
                    TaskCanceledException => StatusCodes.Status504GatewayTimeout,
                    ExternalApiException => (int?)((ExternalApiException)exception).StatusCode,
                    _ => StatusCodes.Status500InternalServerError
                },
                Title = exception switch
                {
                    HttpRequestException => "Bad Gateway",
                    TaskCanceledException => "Gateway Timeout",
                    ExternalApiException => ((ExternalApiException)exception).ErrorTitle,
                    _ => "Internal Server Error"
                },
                Detail = exception switch
                {
                    HttpRequestException => exception.Message,
                    TaskCanceledException => exception.Message,
                    ExternalApiException => ((ExternalApiException)exception).ErrorDetail,
                    _ => "Please try again later."
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, token);

            return true;
        }
    }
}
