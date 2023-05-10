#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Modularity.Plugins
{
	public class TypePlugInSource : IPlugInSource
	{
		private readonly Type[] _moduleTypes;

		public TypePlugInSource(params Type[] moduleTypes)
		{
			_moduleTypes = moduleTypes ?? Type.EmptyTypes;
		}

		public IEnumerable<Type> GetModules()
		{
			return _moduleTypes;
		}
	}
}