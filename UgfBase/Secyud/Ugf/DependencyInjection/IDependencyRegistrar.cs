#region

using System;
using System.Reflection;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyRegistrar
    {
        void AddAssembly(Assembly assembly);

        void AddTypes(params Type[] types);

        void AddType(Type type);

        void AddType<T>();

        void RegisterInstance(Type type, object instance);

        void RegisterInstance<T>(T instance);

        void Register<T, TExposed>(DependencyLifeTime lifeTime = DependencyLifeTime.Singleton) 
            where T:TExposed;

        void RegisterCustom<T, TExposed>(
            IDependencyConstructor constructor,
            DependencyLifeTime lifeTime = DependencyLifeTime.Singleton)
            where T:TExposed;
    }
}