#region

using System;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
	public class CustomContainer<TObject> : ObjectContainer<TObject> where TObject : Object
	{
		private readonly Func<TObject> _getter;

		public CustomContainer(Func<TObject> getter)
		{
			_getter = getter;
		}

		protected override TObject GetObject() => _getter();
	}
}