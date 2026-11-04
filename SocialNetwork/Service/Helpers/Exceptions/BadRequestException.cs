using System.Net;

namespace Service.Helpers.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, HttpStatusCode.BadRequest, innerException)
        {
        }
    }
}
