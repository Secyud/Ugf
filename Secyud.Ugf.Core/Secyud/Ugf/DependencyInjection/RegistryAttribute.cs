﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RegistryAttribute : Attribute
    {
        internal static readonly RegistryAttribute Singleton =
            new()
            {
                IncludeDefaults = true,
                IncludeSelf = true,
                LifeTime = DependencyLifeTime.Singleton
            };

        internal static readonly RegistryAttribute Transient =
            new()
            {
                IncludeDefaults = true,
                IncludeSelf = true,
                LifeTime = DependencyLifeTime.Transient
            };

        private Type _dependScope;

        public Type DependScope
        {
            get => _dependScope;
            set
            {
                _dependScope = value;
                LifeTime = value is null ? LifeTime : DependencyLifeTime.Scoped;
            }
        }

        public DependencyLifeTime LifeTime { get; set; }

        public bool IncludeDefaults { get; set; } = true;

        public bool IncludeSelf { get; set; } = true;
        public Type[] ServiceTypes { get; }

        public RegistryAttribute(params Type[] serviceTypes)
        {
            ServiceTypes = serviceTypes;
        }

        public IEnumerable<Type> GetExposedServiceTypes(Type targetType)
        {
            List<Type> serviceList = ServiceTypes.ToList();

#if DEBUG
            foreach (Type type in serviceList)
            {
                if (!type.IsAssignableFrom(targetType))
                {
                    UgfLogger.LogError($"Please check exposed type for {targetType}" +
                                       $"{type} is not a valid exposed type!");
                }
            }
#endif

            if (IncludeDefaults)
                foreach (Type type in GetDefaultServices(targetType))
                    serviceList.AddIfNotContains(type);

            if (IncludeSelf)
                serviceList.AddIfNotContains(targetType);

            return serviceList.ToArray();
        }

        private static List<Type> GetDefaultServices(Type type)
        {
            List<Type> serviceTypes = new();

            foreach (Type interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                string interfaceName = interfaceType.Name;

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