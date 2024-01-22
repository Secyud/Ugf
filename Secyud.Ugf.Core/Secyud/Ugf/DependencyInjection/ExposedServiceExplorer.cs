#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public static class ExposedServiceExplorer
    {
        public static List<Type> GetExposedServices(Type type)
        {
            return type
                .GetCustomAttributes(true)
                .OfType<RegistryAttribute>()
                .DefaultIfEmpty(RegistryAttribute.Singleton)
                .SelectMany(p => p.GetExposedServiceTypes(type))
                .Distinct()
                .ToList();
        }
    }
}