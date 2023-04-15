#region

using System.Reflection;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace System
{
    public static class UgfTypeExtension
    {
        internal static DependencyLifeTime? GetLifeTimeOrNull(this Type type)
        {
            if (typeof(ITransient).GetTypeInfo().IsAssignableFrom(type))
                return DependencyLifeTime.Transient;

            if (typeof(IScoped).GetTypeInfo().IsAssignableFrom(type))
                return DependencyLifeTime.Scoped;

            if (typeof(ISingleton).GetTypeInfo().IsAssignableFrom(type))
                return DependencyLifeTime.Singleton;

            return null;
        }
    }
}