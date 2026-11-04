using System.Net;

namespace Service.Helpers.Exceptions
{
    public abstract class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected AppException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected AppException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
