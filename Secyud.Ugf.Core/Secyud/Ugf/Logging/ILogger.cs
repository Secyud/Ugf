using System;

namespace Secyud.Ugf.Logging
{
    public interface ILogger
    {
        void LogException(Exception exception);
        void Log(object message);
        void LogWarning(object message);
        void LogError(object message);
    }
}