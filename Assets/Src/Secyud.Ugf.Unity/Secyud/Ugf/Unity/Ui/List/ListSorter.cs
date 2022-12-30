using System.Collections.Generic;

namespace Secyud.Ugf.Unity.Ui
{
    public class ListSorter:IListSorter
    {
        public List<SortUnit> SortProperty { get; set; } = new();
    }
}