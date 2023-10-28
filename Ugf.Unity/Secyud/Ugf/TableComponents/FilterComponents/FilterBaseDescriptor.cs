#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
	public abstract class FilterBaseDescriptor : ICanBeEnabled
	{
		private bool _enabled = true;

		public string Name { get; set; }

		public virtual string Description => null;

		public virtual IObjectAccessor<Sprite> Icon => null;

		public void SetEnabled(bool value)
		{
			_enabled = value;
		}

		public bool GetEnabled() => _enabled;
	}
}