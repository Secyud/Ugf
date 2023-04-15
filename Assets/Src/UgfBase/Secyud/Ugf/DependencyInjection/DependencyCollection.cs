#region

using System;
using System.Collections.Concurrent;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    internal class DependencyCollection : ConcurrentDictionary<Type, DependencyDescriptor>, IDependencyCollection
    {
    }
}