using System;
using System.Reflection;
using UnityEngine;

namespace Secyud.Ugf.Archiving
{
    public class ConstructorContainer
    {
        private readonly ConstructorInfo _constructor;
        public readonly Type Type;

        public object Construct() => _constructor.Invoke(Array.Empty<object>());
        
        public ConstructorContainer(Type type)
        {
            Type = type;
            _constructor = type.GetConstructor(Type.EmptyTypes);
            if (_constructor is null)
                Debug.LogError($"As type with guid, {type} should have a non-parameter constructor but not!");
        }
    }
}