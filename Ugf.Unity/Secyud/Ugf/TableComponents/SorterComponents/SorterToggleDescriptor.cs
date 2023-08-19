#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.SorterComponents
{
	public abstract class SorterToggleDescriptor< TItem> : ICanBeStated
	{
		public virtual string ShowName => null;
		public virtual string ShowDescription => null;
		public virtual IObjectAccessor<Sprite> ShowIcon => null;
		public bool? Enabled { get; set; }

		public Transform Position { get; set; }
		public abstract int SortValue(TItem target);
	}
}