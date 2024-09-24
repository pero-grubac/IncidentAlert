
namespace IncidentAlert_Management.Exceptions
{
    public class FileEmptyException : CustomException
    {
        public FileEmptyException(string message) : base(message) { }

        public FileEmptyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
