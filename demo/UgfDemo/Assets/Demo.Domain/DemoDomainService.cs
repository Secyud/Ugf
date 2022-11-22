using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Demo
{
    public class DemoDomainService:ISingleton
    {
        private int count = 0;
        
        public void PrintMessage(string message)
        {
            Debug.Log($"ptint {message} {++count} times.");
        }
    }
}