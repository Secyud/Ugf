using System.Collections.Generic;

namespace Secyud.Ugf.Prefabs
{
    public class ListSorter:IListSorter
    {
        public List<SortUnit> SortProperty { get; set; } = new();
    }
}