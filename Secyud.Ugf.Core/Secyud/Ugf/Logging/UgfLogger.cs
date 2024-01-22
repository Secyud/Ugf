using System;

namespace Secyud.Ugf.Logging
{
    public static class UgfLogger
    {
        public static ILogger InnerLogger { get; set; } = DefaultLogger.Instance;

        public static void LogException(Exception exception)
        {
            InnerLogger.LogException(exception);
        }

        public static void Log(object message)
        {
            InnerLogger.Log(message);
        }

        public static void LogWarning(object message)
        {
            InnerLogger.LogWarning(message);
        }

        public static void LogError(object message)
        {
            InnerLogger.LogError(message);
        }
    }
}