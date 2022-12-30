using System.Collections.Generic;

namespace Secyud.Ugf.Unity.Ui
{
    public interface IListSorter
    {
        List<SortUnit> SortProperty { get; set; }
    }
}