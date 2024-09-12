namespace IncidentAlert.Exceptions
{
    public class CustomException : Exception
    {
        private string BaseMassege { get; set; }

        public CustomException(string message) : base(message)
        {
            BaseMassege = message;
        }
        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
            BaseMassege = message;
        }
        public string GetBaseMessage() => BaseMassege;
    }
}
