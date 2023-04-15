#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class FilterRegistrationGroup<TItem> : ICanBeEnabled
    {
        public List<FilterRegistration<TItem>> Filters { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description => null;
        public virtual ISpriteGetter Icon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }

        public bool Enabled = true;
    }
}