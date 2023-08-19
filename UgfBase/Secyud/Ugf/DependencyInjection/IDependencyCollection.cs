#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    internal interface IDependencyCollection : IDictionary<Type, DependencyDescriptor>
    {
    }
}