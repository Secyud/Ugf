using System.Collections.Generic;

namespace Secyud.Ugf.Collections
{
    public abstract class RegistrableCollection<TItem>
    {
        public abstract void Register(TItem item);

        public virtual void RegisterList(List<TItem> items)
        {
            foreach (TItem item in items)
            {
                Register(item);
            }
        }

        public virtual void RegisterList(params TItem[] items)
        {
            foreach (TItem item in items)
            {
                Register(item);
            }
        }
    }
}