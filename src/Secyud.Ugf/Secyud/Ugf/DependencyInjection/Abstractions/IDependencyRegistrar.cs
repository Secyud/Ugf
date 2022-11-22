using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyRegistrar
    {
        void AddAssembly(Assembly assembly);

        void AddTypes(IEnumerable<Type> types);

        void AddType(Type type);
        
        void AddType<T>();

        void AddSingleton(Type type, object instance);

        void AddSingleton<T>(T instance);

        void AddSingleton<T, TExposed>();

        void AddScoped<T, TExposed>();

        void AddTransient<T, TExposed>();
    }
}