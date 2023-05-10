#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Collections
{
	public interface ITypeList : ITypeList<object>
	{
	}

	public interface ITypeList<in TBaseType> : IList<Type>
	{
		void Add<T>() where T : TBaseType;

		bool TryAdd<T>() where T : TBaseType;

		bool Contains<T>() where T : TBaseType;

		void Remove<T>() where T : TBaseType;
	}
}