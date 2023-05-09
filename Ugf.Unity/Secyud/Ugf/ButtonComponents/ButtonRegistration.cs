#region

using Secyud.Ugf.BasicComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.ButtonComponents
{
    public abstract class ButtonRegistration<TItem> : ITriggerable
    {
        public TItem Target { get; set; }
        public abstract void Trigger();
        public abstract string ShowName { get; }
        public virtual string ShowDescription => null;
        public virtual IObjectAccessor<Sprite> ShowIcon => null;

        public virtual void SetButton(SButton button)
        {
        }

        public abstract bool Visible(TItem target);

        public virtual bool Visible() => true;
    }
}