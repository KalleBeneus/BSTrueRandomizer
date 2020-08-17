using System;

namespace BSTrueRandomizer.Exceptions
{
    internal class RandomizationException : Exception
    {
        public RandomizationException(string message) : base(message)
        {
        }
    }
}