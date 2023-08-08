using Secyud.Ugf.RefreshComponents;

namespace Secyud.Ugf.ValidateComponents
{
    public class ValidateService<TService,TItem>:RefreshService<TService,TItem> 
        where TService :ValidateService<TService,TItem>
        where TItem : ValidateItem<TService,TItem>
    {
        public bool Valid { get; set; }
    }
}