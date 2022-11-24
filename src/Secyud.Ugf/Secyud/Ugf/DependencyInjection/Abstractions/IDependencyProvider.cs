using System;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyProvider
    {
        object GetDependency(Type type);

        bool TryGetDependency(Type type, out object dependency);

        T GetDependency<T>()where T : class;

        bool TryGetDependency<T>(out T dependency) where T : class;
    }
}