#region

using Secyud.Ugf.BasicComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.ButtonComponents
{
	public abstract class ButtonDescriptor<TItem> 
	{
		public TItem Target { get; set; }

		public abstract void Invoke();

		public abstract string Name { get; }

		public virtual string Description => null;

		public virtual IObjectAccessor<Sprite> Icon => null;

		public virtual void SetButton(SLabelButton button)
		{
		}

		public abstract bool Visible(TItem target);

		public virtual bool Visible() => true;
	}
}