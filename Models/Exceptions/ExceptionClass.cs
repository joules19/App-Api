namespace api_app.Models
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }

    public class UnauthorizedRequestException : Exception
    {
        public UnauthorizedRequestException(string message) : base(message)
        {
        }
    }
}
