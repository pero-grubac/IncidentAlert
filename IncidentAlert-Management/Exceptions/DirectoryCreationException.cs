
namespace IncidentAlert_Management.Exceptions
{
    public class DirectoryCreationException : CustomException
    {
        public DirectoryCreationException(string message) : base(message) { }

        public DirectoryCreationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
