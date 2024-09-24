
namespace IncidentAlert_Management.Exceptions
{
    public class EntityCanNotBeCreatedException : CustomException
    {
        public EntityCanNotBeCreatedException(string message) : base(message) { }

        public EntityCanNotBeCreatedException(string message, Exception innerException) : base(message, innerException) { }

    }
}
