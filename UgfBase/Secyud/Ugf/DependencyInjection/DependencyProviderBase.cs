#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
	public abstract class DependencyProviderBase : IDependencyProvider
	{
		internal readonly IDependencyCollection DependencyCollection;
		protected readonly Dictionary<Type, object> Instances;
		private static readonly Dictionary<Type, ConstructorDescriptor> Constructors = new();


		internal DependencyProviderBase(
			IDependencyCollection dependencyCollection,
			Dictionary<Type, object> instances)
		{
			DependencyCollection = dependencyCollection;
			Instances = instances;
		}

		public virtual T Get<T>() where T : class
		{
			return Get(typeof(T)) as T;
		}

		public abstract object Get(Type type);

		public DependencyDescriptor GetDescriptor(Type type)
		{
			DependencyCollection.TryGetValue(type, out DependencyDescriptor provider);
			return provider;
		}

		protected object CreateInstance(Type implementationType)
		{
			if (!Constructors.TryGetValue(implementationType, out ConstructorDescriptor constructor))
			{
				ConstructorInfo ci = implementationType.GetConstructors(Og.ConstructFlag).FirstOrDefault();
				if (ci is null)
					throw new UgfException($"Can not find constructor for type {implementationType}.");
				constructor = new ConstructorDescriptor(ci);
				Constructors[implementationType] = constructor;
			}
#if DEBUG
			Debug.Log($"{implementationType} is constructing!");
#endif
			return constructor.Construct(this);
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