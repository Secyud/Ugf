#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Secyud.Ugf.Collections
{
    public class RegistrableList<TItem>
    {
        public readonly List<TItem> Items = new();

        public void Register(TItem item)
        {
            Items.Add(item);
        }

        public void RegisterList(params TItem[] items)
        {
            foreach (TItem item in items) Register(item);
        }
    }
}