#region

using System.Collections.Generic;
using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public sealed class FilterRegistrationGroup<TItem> : ICanBeEnabled
    {
        public bool Enabled = true;

        public List<FilterRegistration<TItem>> Filters { get; set; } = new();
        public string ShowName { get; set; }
        public string ShowDescription => null;
        public IObjectAccessor<Sprite> ShowIcon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }
    }
}