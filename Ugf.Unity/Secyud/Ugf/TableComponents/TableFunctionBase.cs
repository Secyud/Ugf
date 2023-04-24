#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public abstract class TableFunctionBase<TItem> : ISingleton, IHasFilterGroups<TItem>, IHasSorters<TItem>
    {
        private readonly List<FilterRegistrationGroup<TItem>> _filterGroups = new();
        private readonly List<ISorterRegistration<TItem>> _sorters = new();

        public IEnumerable<FilterRegistrationGroup<TItem>> FilterGroups => _filterGroups;
        public IEnumerable<ISorterRegistration<TItem>> Sorters => _sorters;

        public void RegisterFilterGroup(FilterRegistrationGroup<TItem> filterGroup)
        {
            _filterGroups.Add(filterGroup);
        }

        public void RegisterFilterGroups(params FilterRegistrationGroup<TItem>[] filterGroups)
        {
            foreach (var filterGroup in filterGroups)
                RegisterFilterGroup(filterGroup);
        }

        public void RegisterSorter(ISorterRegistration<TItem> sorter)
        {
            _sorters.Add(sorter);
        }

        public void RegisterSorters(params ISorterRegistration<TItem>[] sorters)
        {
            foreach (var sorter in sorters)
                RegisterSorter(sorter);
        }
    }
}