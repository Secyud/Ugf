#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents
{
	[Registry]
	public abstract class TableFunctionBase<TItem> :  IHasFilterGroups<TItem>, IHasSorters<TItem>
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
			foreach (FilterRegistrationGroup<TItem> filterGroup in filterGroups)
				RegisterFilterGroup(filterGroup);
		}

		public void RegisterSorter(ISorterRegistration<TItem> sorter)
		{
			_sorters.Add(sorter);
		}

		public void RegisterSorters(params ISorterRegistration<TItem>[] sorters)
		{
			foreach (ISorterRegistration<TItem> sorter in sorters)
				RegisterSorter(sorter);
		}
	}
}