#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
	public abstract class FilterBaseDescriptor : ICanBeEnabled
	{
		private bool _enabled = true;

		public virtual string ShowName => null;

		public virtual string ShowDescription => null;

		public virtual IObjectAccessor<Sprite> ShowIcon => null;

		public void SetEnabled(bool value)
		{
			_enabled = value;
		}

		public bool GetEnabled() => _enabled;
	}
}