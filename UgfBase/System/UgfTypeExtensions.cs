#region

using Secyud.Ugf.DependencyInjection;
using System.Reflection;

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

	}
}