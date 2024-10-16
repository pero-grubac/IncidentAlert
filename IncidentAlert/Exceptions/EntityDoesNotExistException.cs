﻿namespace IncidentAlert.Exceptions
{
    public class EntityDoesNotExistException : CustomException
    {
        public EntityDoesNotExistException(string message) : base(message) { }

        public EntityDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }

    }
}
