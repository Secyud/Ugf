using System;
using UnityEngine;

namespace Secyud.Ugf.TabComponents
{
    public class TabActionItem<TService, TItem> : TabItem<TService, TItem>
        where TService : TabService<TService, TItem>
        where TItem : TabItem<TService, TItem>
    {
        private readonly Action _action;

        public TabActionItem(string name, GameObject gameObject, Action action) 
            : base( name, gameObject)
        {
            _action = action;
        }

        public override void Refresh()
        {
            _action?.Invoke();
        }
    }
}