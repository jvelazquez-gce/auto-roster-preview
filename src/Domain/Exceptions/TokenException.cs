﻿using System;

namespace Domain.Exceptions
{
    public class TokenException : Exception
    {
        public TokenException() { }
        public TokenException(string message) : base(message) { }
    }
}
