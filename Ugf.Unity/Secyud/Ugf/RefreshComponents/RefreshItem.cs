using System;
using System.Collections.Generic;

namespace Secyud.Ugf.RefreshComponents
{
    public abstract class RefreshItem<TService,TItem>
        where TService : RefreshService<TService,TItem>
        where TItem : RefreshItem<TService,TItem>
    {
        public readonly TService Service;
        public string Name { get; }
        protected RefreshItem( string name)
        {
            Name = name;
            Service = U.Get<TService>();
            if (this is TItem item)
                Service.RefreshItems[Name] = item;
        }

        public virtual void Remove()
        {
            Dictionary<string, TItem> dict = Service.RefreshItems;
            if (dict.TryGetValue(Name, out TItem item) && item == this)
                dict.Remove(Name);
        }

        public abstract void Refresh();
    }
}