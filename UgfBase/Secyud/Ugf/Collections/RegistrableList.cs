#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Collections
{
    public class RegistrableList<TItem>:RegistrableCollection<TItem>
    {
        public readonly List<TItem> Items = new();

        public override void Register(TItem item)
        {
            Items.Add(item);
        }
    }
}