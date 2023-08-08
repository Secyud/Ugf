using Secyud.Ugf.RefreshComponents;

namespace Secyud.Ugf.ValidateComponents
{
    public abstract class ValidateItem<TService,TItem>:RefreshItem<TService,TItem>
        where TService :ValidateService<TService,TItem>
        where TItem : ValidateItem<TService,TItem>
    {
        protected ValidateItem(string name) : base( name)
        {
        }

        protected abstract bool CheckValid();

        public override void Refresh()
        {
            if (!CheckValid())
            {
                Service.Valid = false;
            }
        }
    }
}