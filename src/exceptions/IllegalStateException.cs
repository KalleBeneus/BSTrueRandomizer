using System;

namespace BSTrueRandomizer.Exceptions
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException(string message) : base(message)
        {
        }
    }
}