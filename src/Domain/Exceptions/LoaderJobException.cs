using System;

namespace Domain.Exceptions
{
    public class LoaderJobException : Exception 
    {
        public LoaderJobException() {}
        public LoaderJobException(string message) : base(message) {}
    }
}
