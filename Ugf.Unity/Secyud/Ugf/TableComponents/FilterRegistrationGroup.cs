#region

using System.Collections.Generic;
using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class FilterRegistrationGroup<TItem> : ICanBeEnabled
    {
        public bool Enabled = true;
        public List<FilterRegistration<TItem>> Filters { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description => null;
        public virtual SpriteContainer Icon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }
    }
}