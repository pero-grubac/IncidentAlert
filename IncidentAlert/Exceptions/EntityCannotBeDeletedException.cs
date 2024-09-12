namespace IncidentAlert.Exceptions
{
    public class EntityCannotBeDeletedException : Exception
    {
        public EntityCannotBeDeletedException() { }
        public EntityCannotBeDeletedException(string message) : base(message) { }
        public EntityCannotBeDeletedException(string message, Microsoft.EntityFrameworkCore.DbUpdateException ex) : base(message) { }

    }
}
