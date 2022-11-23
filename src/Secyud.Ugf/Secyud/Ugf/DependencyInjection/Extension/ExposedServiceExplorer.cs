using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.DependencyInjection;

public static class ExposedServiceExplorer
{
    private static readonly ExposeTypeAttribute DefaultExposeTypeAttribute =
        new()
        {
            IncludeDefaults = true,
            IncludeSelf = true
        };

    public static List<Type> GetExposedServices(Type type)
    {
        return type
            .GetCustomAttributes(true)
            .OfType<IExposedTypesProvider>()
            .DefaultIfEmpty(DefaultExposeTypeAttribute)
            .SelectMany(p => p.GetExposedServiceTypes(type))
            .Distinct()
            .ToList();
    }
}