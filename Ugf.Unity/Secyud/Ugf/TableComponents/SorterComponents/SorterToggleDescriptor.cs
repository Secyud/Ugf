#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.SorterComponents
{
	public abstract class SorterToggleDescriptor< TItem> : ICanBeStated
	{
		public virtual string Name => null;
		public virtual string Description => null;
		public virtual IObjectAccessor<Sprite> Icon => null;
		public bool? Enabled { get; set; }

		public Transform Position { get; set; }
		public abstract int SortValue(TItem target);
	}
}