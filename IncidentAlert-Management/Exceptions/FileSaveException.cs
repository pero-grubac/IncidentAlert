
namespace IncidentAlert_Management.Exceptions
{
    public class FileSaveException : CustomException
    {
        public FileSaveException(string message) : base(message) { }

        public FileSaveException(string message, Exception innerException) : base(message, innerException) { }
    }
}
