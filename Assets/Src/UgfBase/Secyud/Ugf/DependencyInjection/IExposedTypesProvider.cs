#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public interface IExposedTypesProvider
    {
        IEnumerable<Type> GetExposedServiceTypes(Type targetType);
    }
}