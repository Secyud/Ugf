using System;
using System.Collections.Generic;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IExposedTypesProvider
    {
        IEnumerable<Type> GetExposedServiceTypes(Type targetType);
    }
}