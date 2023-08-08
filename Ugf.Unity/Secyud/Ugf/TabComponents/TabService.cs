using Secyud.Ugf.RefreshComponents;

namespace Secyud.Ugf.TabComponents
{
    public class TabService<TService,TItem> : RefreshService<TService,TItem>
        where TService : TabService<TService,TItem>
        where TItem : TabItem<TService,TItem>
    {
    }
}