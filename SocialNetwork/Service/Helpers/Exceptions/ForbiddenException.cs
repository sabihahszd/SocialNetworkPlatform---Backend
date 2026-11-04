using System.Net;

namespace Service.Helpers.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, HttpStatusCode.Forbidden, innerException)
        {
        }
    }
}
