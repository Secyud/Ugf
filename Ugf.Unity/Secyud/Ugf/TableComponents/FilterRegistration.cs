#region

using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public abstract class FilterRegistration<TTarget> : ICanBeEnabled
    {
        public bool Enabled = true;
        public virtual string ShowName => null;
        public virtual string ShowDescription => null;
        public virtual IObjectAccessor<Sprite> ShowIcon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }

        public bool GetEnabled() => Enabled;

        public abstract bool Filter(TTarget target);
    }
}