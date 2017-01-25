using System;

namespace SecurityLib
{
    public class SecureCardException : Exception
    {
        /// <summary>
        /// Exception for secure Card
        /// </summary>
        /// <param name="message"></param>
        public SecureCardException(string message)
            : base(message)
        {
        }
    }
}