using System;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyProvider
    {
        object Get(Type type);

        T Get<T>()where T : class;
    }
}