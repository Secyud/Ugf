#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public class ExposeTypeAttribute : Attribute, IExposedTypesProvider
    {
        public ExposeTypeAttribute(params Type[] serviceTypes)
        {
            ServiceTypes = serviceTypes ?? Type.EmptyTypes;
        }

        public Type[] ServiceTypes { get; }

        public bool IncludeDefaults { get; set; } = true;

        public bool IncludeSelf { get; set; } = true;

        public IEnumerable<Type> GetExposedServiceTypes(Type targetType)
        {
            var serviceList = ServiceTypes.ToList();

            if (IncludeDefaults)
                foreach (var type in GetDefaultServices(targetType))
                    serviceList.AddIfNotContains(type);

            if (IncludeSelf)
                serviceList.AddIfNotContains(targetType);

            return serviceList.ToArray();
        }

        private static List<Type> GetDefaultServices(Type type)
        {
            var serviceTypes = new List<Type>();

            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                var interfaceName = interfaceType.Name;

                if (interfaceType.IsGenericType)
                    interfaceName = interfaceType.Name[..interfaceType.Name.IndexOf('`')];

                if (interfaceName.StartsWith("I"))
                    interfaceName = interfaceName[1..];

                if (type.Name.EndsWith(interfaceName))
                    serviceTypes.Add(interfaceType);
            }

            return serviceTypes;
        }
    }
}