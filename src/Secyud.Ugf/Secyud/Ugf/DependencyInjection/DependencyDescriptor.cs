using System;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    internal class DependencyDescriptor
    {
        private DependencyDescriptor()
        {
        }

        public Type ImplementationType { get; private set; }
        public DependencyLifeTime DependencyLifeTime { get; private set; }

        internal static DependencyDescriptor Describe(Type implementationType, DependencyLifeTime lifeTime)
        {
            return new DependencyDescriptor
            {
                ImplementationType = implementationType,
                DependencyLifeTime = lifeTime
            };
        }

        internal object CreateInstance(IDependencyProvider serviceProvider)
        {
            var constructors = ImplementationType.GetConstructors(
                BindingFlags.Public | BindingFlags.Instance);

            var parameters = constructors[0]
                .GetParameters()
                .Select(u => serviceProvider.GetDependency(u.ParameterType)).ToArray();

            return Activator.CreateInstance(ImplementationType, parameters);
        }
    }
}