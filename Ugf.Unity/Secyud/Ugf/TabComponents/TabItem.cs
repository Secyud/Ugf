using Secyud.Ugf.RefreshComponents;
using UnityEngine;

namespace Secyud.Ugf.TabComponents
{
    public abstract class TabItem<TService, TItem> : RefreshItem<TService, TItem>
        where TService : TabService<TService, TItem>
        where TItem : TabItem<TService, TItem>
    {
        public GameObject GameObject { get; }

        protected TabItem(string name, GameObject gameObject) : base( name)
        {
            GameObject = gameObject;
        }
    }
}