#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public abstract class ListServiceBase<TItem> :
        ISingleton,
        IHasFilterGroups<TItem>,
        IHasSorters<TItem>
    {
        protected readonly List<FilterRegistrationGroup<TItem>> FilterGroups = new();
        protected readonly List<ISorterRegistration<TItem>> Sorters = new();

        public virtual void RegisterFilterGroup(FilterRegistrationGroup<TItem> itemFilterGroup)
        {
            FilterGroups.Add(itemFilterGroup);
        }

        public virtual IEnumerable<FilterRegistrationGroup<TItem>> GetFilterGroups()
        {
            return FilterGroups;
        }

        public virtual void RegisterSorter(ISorterRegistration<TItem> itemSorter)
        {
            Sorters.Add(itemSorter);
        }

        public virtual IEnumerable<ISorterRegistration<TItem>> GetSorters()
        {
            return Sorters;
        }
    }
}