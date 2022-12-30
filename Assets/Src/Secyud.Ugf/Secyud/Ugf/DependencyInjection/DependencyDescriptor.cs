using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Secyud.Ugf.DependencyInjection
{
    internal class DependencyDescriptor
    {
        private DependencyDescriptor()
        {
        }

        public Type ImplementationType { get; private set; }
        public DependencyLifeTime DependencyLifeTime { get; private set; }
        
        public Func<object> InstanceAccessor { get; set; }

        internal static DependencyDescriptor Describe(Type implementationType, DependencyLifeTime lifeTime,Func<object> instanceAccessor)
        {
            return new DependencyDescriptor
            {
                ImplementationType = implementationType,
                DependencyLifeTime = lifeTime,
                InstanceAccessor = instanceAccessor
            };
        }
    }
}