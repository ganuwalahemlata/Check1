using System;

namespace SecurityLib
{
    public class StringEncryptorException : Exception
    {
        /// <summary>
        /// Exception Handler for StringEncryptor
        /// </summary>
        /// <param name="message"></param>
        public StringEncryptorException(string message)
            : base(message)
        {
        }
    }
}