#region

using System;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyProvider
    {
        object Get(Type type);

        T Get<T>() where T : class;
        object TryGet(Type type);
        T TryGet<T>() where T : class;
    }
}