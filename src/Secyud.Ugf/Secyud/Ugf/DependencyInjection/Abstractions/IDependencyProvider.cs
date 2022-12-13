using System;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyProvider
    {
        object GetDependency(Type type);

        T GetDependency<T>()where T : class;
    }
}