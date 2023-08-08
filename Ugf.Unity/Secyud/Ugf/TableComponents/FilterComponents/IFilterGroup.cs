using System.Collections.Generic;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public interface IFilterGroup<TFilter> where TFilter:FilterBase<TFilter>
    {
        List<TFilter> Filters { get;  }
        void RefreshTable();
    }
}