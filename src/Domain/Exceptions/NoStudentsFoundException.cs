using System;

namespace Domain.Exceptions
{
    public class NoStudentsFoundException : Exception 
    {
        public NoStudentsFoundException() {}
        public NoStudentsFoundException(string message) : base(message) {}
    }
}
