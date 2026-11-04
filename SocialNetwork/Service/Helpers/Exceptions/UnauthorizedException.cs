using System.Net;

namespace Service.Helpers.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "You need to sign in.")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, HttpStatusCode.Unauthorized, innerException)
        {
        }
    }
}
