#region

using System;
using System.Runtime.Serialization;

#endregion

namespace Secyud.Ugf
{
    public class UgfException : Exception
    {
        public UgfException()
        {
        }

        public UgfException(string message)
            : base(message)
        {
        }

        public UgfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UgfException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}