using System;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Container
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