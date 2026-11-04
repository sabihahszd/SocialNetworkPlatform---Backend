using System.Net;

namespace Service.Helpers.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict)
        {
        }

        public ConflictException(string message, Exception innerException)
            : base(message, HttpStatusCode.Conflict, innerException)
        {
        }
    }
}
