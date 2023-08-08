using System;

namespace Secyud.Ugf.RefreshComponents
{
    public class RefreshActionItem<TService,TItem>:RefreshItem<TService,TItem>
        where TService : RefreshService<TService,TItem>
        where TItem : RefreshItem<TService,TItem>
    {
        private readonly Action _action;

        public RefreshActionItem(string name,Action action) : base(name)
        {
            _action = action;
        }

        public override void Refresh()
        {
            _action.Invoke();
        }
    }
}