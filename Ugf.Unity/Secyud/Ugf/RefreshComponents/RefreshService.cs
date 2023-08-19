using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.RefreshComponents
{
    public abstract class RefreshService<TService,TItem> : IRegistry
    where TService : RefreshService<TService,TItem>
    where TItem : RefreshItem<TService,TItem>
    {
        public virtual Dictionary<string, TItem> RefreshItems { get; } = new();

        public virtual void Refresh()
        {
            foreach (TItem item in RefreshItems.Values)
                item.Refresh();
        }
    }
}