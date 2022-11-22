using System;
using System.Collections.Generic;

namespace Secyud.Ugf.DependencyInjection
{
    internal interface IDependencyCollection : IDictionary<Type, DependencyDescriptor>
    {
    }
}