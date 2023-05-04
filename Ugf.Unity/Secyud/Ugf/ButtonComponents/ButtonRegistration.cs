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
        public abstract string Name { get; }
        public virtual string Description => null;
        public virtual SpriteContainer Icon => null;

        public virtual void SetButton(SButton button)
        {
        }

        public abstract bool Visible(TItem target);
    }
}