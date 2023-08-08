using System;

namespace Secyud.Ugf.ValidateComponents
{
    public abstract class ValidateActionItem<TService, TItem> : ValidateItem<TService, TItem>
        where TService : ValidateService<TService, TItem>
        where TItem : ValidateItem<TService, TItem>
    {
        private readonly Func<bool> _action;

        protected ValidateActionItem(string name, Func<bool> action)
            : base(name)
        {
            _action = action;
        }

        protected override bool CheckValid()
        {
            return _action.Invoke();
        }
    }
}