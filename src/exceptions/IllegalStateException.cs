using System;

namespace BSTrueRandomizer.Exceptions
{
    internal class IllegalStateException : Exception
    {
        public IllegalStateException(string message) : base(message)
        {
        }
    }
}