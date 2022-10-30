using System;
using System.Runtime.Serialization;

namespace Secyud.Ugf;

public class UgfShutdownException : UgfException
{
    public UgfShutdownException()
    {
    }

    public UgfShutdownException(string message)
        : base(message)
    {
    }

    public UgfShutdownException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UgfShutdownException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}