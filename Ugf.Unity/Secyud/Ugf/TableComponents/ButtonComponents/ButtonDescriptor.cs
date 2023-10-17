#region

using Secyud.Ugf.BasicComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.ButtonComponents
{
	public abstract class ButtonDescriptor<TItem> : ITriggerable
	{
		public TItem Target { get; set; }

		public abstract void Invoke();

		public abstract string ShowName { get; }

		public virtual string ShowDescription => null;

		public virtual IObjectAccessor<Sprite> ShowIcon => null;

		public virtual void SetButton(SLabelButton button)
		{
		}

		public abstract bool Visible(TItem target);

		public virtual bool Visible() => true;
	}
}