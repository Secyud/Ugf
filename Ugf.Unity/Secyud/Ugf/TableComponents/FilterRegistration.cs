#region

using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public abstract class FilterRegistration<TTarget> : ICanBeEnabled
    {
        public bool Enabled = true;
        public virtual string Name => null;
        public virtual string Description => null;
        public virtual SpriteContainer Icon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }

        public abstract bool Filter(TTarget target);
    }
}