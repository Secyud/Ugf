using System;

namespace Secyud.Ugf.DependencyInjection;

public abstract class DependencyProviderBase : IDependencyProvider
{
    public bool TryGetDependency<T>(out T dependency)
    {
        if (TryGetDependency(typeof(T), out var dependencyObj))
        {
            dependency = (T)dependencyObj;
            return true;
        }

        dependency = default;
        return false;
    }

    public abstract bool TryGetDependency(Type type, out object dependency);

    public virtual T GetDependency<T>()
    {
        return (T)GetDependency(typeof(T));
    }

    public abstract object GetDependency(Type type);
    internal abstract DependencyDescriptor GetDescriptor(Type type);
}