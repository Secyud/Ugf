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

		void AddSingleton(Type type, object instance);

		void AddSingleton<T>(T instance);

		void AddSingleton<T, TExposed>();

		void AddScoped<T, TExposed>();

		void AddTransient<T, TExposed>();

		void AddCustom<T, TExposed>(Func<object> instanceAccessor);
	}
}