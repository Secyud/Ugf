using System;
using System.Collections.Generic;

namespace Secyud.Ugf.DependencyInjection
{
    public class ExposedTypesContext : IExposedTypesContext
    {
        public ExposedTypesContext(Type implementationType, List<Type> exposedTypes)
        {
            Thrower.IfNull(implementationType);
            Thrower.IfNull(exposedTypes);

            ImplementationType = implementationType;
            ExposedTypes = exposedTypes;
        }

        public Type ImplementationType { get; }

        public List<Type> ExposedTypes { get; }
    }
}