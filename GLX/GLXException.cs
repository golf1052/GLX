using System;

namespace GLX
{
    /// <summary>
    /// Represents errors that occur while using GLX
    /// </summary>
    public class GLXException : Exception
    {
        /// <summary>
        /// Creates a new GLXException.
        /// </summary>
        public GLXException() : base()
        {
        }

        /// <summary>
        /// Creates a new GLXException with a specified error message.
        /// </summary>
        /// <param name="message"></param>
        public GLXException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new GLXException with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public GLXException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
