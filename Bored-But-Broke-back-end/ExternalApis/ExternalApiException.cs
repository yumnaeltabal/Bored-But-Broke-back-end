using System.Net;

namespace Bored_But_Broke_back_end.ExternalApis
{
    public class ExternalApiException : Exception
    {
        public HttpStatusCode? StatusCode { get; }
        public string? ErrorTitle { get; }
        public string? ErrorDetail { get; }
        public ExternalApiException(HttpStatusCode? statusCode = null, string? errorTitle = null, 
            string? errorDetail = null, string? message = null) : base(message) 
        { 
            StatusCode = statusCode;
            ErrorTitle = errorTitle;
            ErrorDetail = errorDetail;
        }
    }
}
