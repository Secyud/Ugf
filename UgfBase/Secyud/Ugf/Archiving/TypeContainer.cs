using System;
using System.Reflection;
using UnityEngine;

namespace Secyud.Ugf.Archiving
{
	public class TypeContainer
	{
		public readonly ConstructorInfo Constructor;
		public readonly Type Type;

		public object Construct() => Constructor.Invoke(Array.Empty<object>());

		public TypeContainer(Type type)
		{
			Type = type;
			Constructor = type.GetConstructor(Type.EmptyTypes);
			if (Constructor is null)
				Debug.LogError($"As type with guid, {type} should have a non-parameter constructor but not!");
		}

		public TypeContainer(Type type, ConstructorInfo constructor)
		{
			Type = type;
			Constructor = constructor;
		}
	}
}