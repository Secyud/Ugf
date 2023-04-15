#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public interface IHasSorters<in TItem>
    {
        public IEnumerable<ISorterRegistration<TItem>> GetSorters();
    }
}