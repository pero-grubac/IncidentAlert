namespace IncidentAlert_Management.Exceptions
{
    public class EntityCannotBeDeletedException : CustomException
    {
        public EntityCannotBeDeletedException(string message) : base(message) { }
        public EntityCannotBeDeletedException(string message, Exception ex) : base(message) { }
    }
}
