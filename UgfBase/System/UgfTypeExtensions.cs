#region

using System.Reflection;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace System
{
    public static class UgfTypeExtensions
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

        
        public static Guid GetTypeId(this object type)
        {
            return type.GetType().GetId();
        }
        public static Guid GetId(this Type type)
        {
            return type.GetCustomAttribute<TypeIdAttribute>()?.Id ?? Guid.Empty;
        }
    }
}