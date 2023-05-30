#region

using System;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
	public class DependencyDescriptor
	{
		private DependencyDescriptor()
		{
		}

		public Type ImplementationType { get; private set; }

		public DependencyLifeTime DependencyLifeTime { get; private set; }

		public Func<object> InstanceAccessor { get; set; }

		internal static DependencyDescriptor Describe(Type implementationType, DependencyLifeTime lifeTime,
			Func<object> instanceAccessor)
		{
			return new DependencyDescriptor
			{
				ImplementationType = implementationType,
				DependencyLifeTime = lifeTime,
				InstanceAccessor = instanceAccessor
			};
		}
	}
}