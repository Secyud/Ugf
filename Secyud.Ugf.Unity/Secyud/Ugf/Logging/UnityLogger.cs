using System;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Logging
{
    public class UnityLogger:ILogger,IRegistry
    {
        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public void Log(object message)
        {
            Debug.Log(message);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}