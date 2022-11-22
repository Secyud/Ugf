using System;
using System.Collections.Concurrent;

namespace Secyud.Ugf.DependencyInjection
{
    internal class DependencyCollection : ConcurrentDictionary<Type, DependencyDescriptor>, IDependencyCollection
    {
    }
}