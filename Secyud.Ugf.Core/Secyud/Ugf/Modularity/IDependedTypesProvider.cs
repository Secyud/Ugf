using System;

namespace Secyud.Ugf.Modularity
{
    public interface IDependedTypesProvider
    {
        Type[] DependedTypes { get; }
    }
}