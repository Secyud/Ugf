#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public abstract class ButtonRegistration<TItem> : ITriggerable
    {
        public virtual void SetButton(SButton button)
        {
            
        }

        public virtual bool Visible(TItem target) => true;
        public abstract void Trigger();
        public abstract string Name { get; }
        public virtual string Description => null;
        public virtual ISpriteGetter Icon => null;
        public TItem Target { get; set; }
    }
}