using System;

namespace NamuDarbas.Exceptions
{
    public class InvalidAlgorithmException : Exception
    {
        public InvalidAlgorithmException() : base("The specified algorithm is not valid.")
        {
        }

        public InvalidAlgorithmException(string message) : base(message)
        {
        }

        public InvalidAlgorithmException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException() : base("The specified key was not found in the database.")
        {
        }

        public KeyNotFoundException(string message) : base(message)
        {
        }

        public KeyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}