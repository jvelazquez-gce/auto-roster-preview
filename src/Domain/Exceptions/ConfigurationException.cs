using System;

namespace Domain.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException() { }
        public ConfigurationException(string message) : base(message) { }
    }
}
