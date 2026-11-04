using System.Net;

namespace Service.Helpers.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message = "The requested resource was not found.")
            : base(message, HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, HttpStatusCode.NotFound, innerException)
        {
        }
    }
}
