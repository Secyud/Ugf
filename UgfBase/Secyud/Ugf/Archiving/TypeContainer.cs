#region

using System;
using System.Reflection;

#endregion

namespace Secyud.Ugf.Archiving
{
	public class TypeContainer
	{
		public readonly ConstructorInfo Constructor;
		public readonly Type Type;

		public object Construct()
		{
			return Constructor?.Invoke(Array.Empty<object>());
		}

		public TypeContainer(Type type)
		{
			Type = type;
			Constructor = type.GetConstructor(Og.ConstructFlag, null, Type.EmptyTypes, null);
		}
	}
}