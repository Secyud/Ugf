using System;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Logging
{
    public class DefaultLogger : ILogger,IRegistry
    {
        public static DefaultLogger Instance { get; } = new();
        private DefaultLogger() { }
        
        public void LogException(Exception exception)
        {
            Console.WriteLine(exception);
        }

        public void Log(object message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(object message)
        {
            Console.WriteLine(message);
        }

        public void LogError(object message)
        {
            Console.Error.WriteLine(message);
        }
    }
}