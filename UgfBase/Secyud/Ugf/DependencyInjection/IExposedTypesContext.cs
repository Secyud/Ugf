#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public interface IExposedTypesContext
    {
        Type ImplementationType { get; }

        List<Type> ExposedTypes { get; }
    }
}