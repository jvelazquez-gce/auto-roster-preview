using System;

namespace Domain.Exceptions
{
    public class InvalidJobModeException : Exception 
    {
        public InvalidJobModeException() {}
        public InvalidJobModeException(string message) : base(message) {}
    }
}
