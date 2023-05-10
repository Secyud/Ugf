#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
	public abstract class DependencyProviderBase : IDependencyProvider
	{
		internal readonly IDependencyCollection DependencyCollection;
		protected readonly ConcurrentDictionary<Type, object> Instances;

		internal DependencyProviderBase(
			IDependencyCollection dependencyCollection,
			ConcurrentDictionary<Type, object> instances)
		{
			DependencyCollection = dependencyCollection;
			Instances = instances;
		}

		public virtual T Get<T>() where T : class
		{
			return Get(typeof(T)) as T;
		}

		public abstract object Get(Type type);

		internal DependencyDescriptor GetDescriptor(Type type)
		{
			DependencyCollection.TryGetValue(type, out DependencyDescriptor provider);
			return provider;
		}

		protected object CreateInstance(Type implementationType)
		{
			var constructor = implementationType.GetConstructors(Og.ConstructFlag).FirstOrDefault();

			if (constructor is null)
				throw new UgfException($"Can not find constructor for type {implementationType}.");

			var parameters = constructor
				.GetParameters()
				.Select(u => Get(u.ParameterType)).ToArray();

#if DEBUG
			Debug.Log($"{implementationType} is constructing!");
#endif

			return constructor.Invoke(parameters);
		}

		protected object GetInstance(Type implementationType)
		{
			if (!Instances.TryGetValue(implementationType, out object instance))
			{
				instance = CreateInstance(implementationType);
				Instances[implementationType] = instance;
			}
			return instance;
		}
	}
}