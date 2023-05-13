#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public abstract class FunctionalTableHelper<TItem, TCell, TListService> : TableHelper<TItem, TCell>
		where TListService : TableFunctionBase<TItem>
		where TCell : MonoBehaviour
	{
		protected IList<TItem> FilteredItems;
		protected List<FilterRegistrationGroup<TItem>> FilterGroups;
		protected List<Tuple<ISorterRegistration<TItem>, Transform>> Sorters;
		protected IList<TItem> TotalItems;

		public virtual void OnInitialize(FunctionalTable table, TCell cellTemplate, IList<TItem> showItems)
		{
			TotalItems = showItems;
			FilteredItems = TotalItems;

			TListService service = Og.Get<TListService>();

			FilterGroups = service.FilterGroups.ToList();

			for (int i = 0; i < table.FixedContent.transform.childCount; i++)
				Object.Destroy(table.FixedContent.transform.GetChild(i).gameObject);

			foreach (FilterRegistrationGroup<TItem> filterGroup in FilterGroups)
			{
				FilterGroup fgc = table.FilterGroupTemplate.Create(
					table.FixedContent.transform, table, filterGroup
				);
				table.FilterGroups.Add(fgc);
				fgc.ChildFilters.AddRange(filterGroup.Filters);
			}

			table.FixedContent.enabled = true;

			Sorters = new List<Tuple<ISorterRegistration<TItem>, Transform>>();

			for (int i = 0; i < table.SortableContent.transform.childCount; i++)
				Object.Destroy(table.SortableContent.transform.GetChild(i).gameObject);

			foreach (ISorterRegistration<TItem> sorter in service.Sorters)
			{
				Sorter s = table.SorterTemplate.Create(table.SortableContent.transform, table, sorter);

				Sorters.Add(new Tuple<ISorterRegistration<TItem>, Transform>(sorter, s.transform));
			}
			table.SortableContent.enabled = true;

			base.OnInitialize(table, cellTemplate, showItems);
		}

		public override void ApplyFilter()
		{
			if (FilterGroups is null) return;

			IEnumerable<IEnumerable<FilterRegistration<TItem>>> filterGroups =
				FilterGroups
					.Where(u => u.GetEnabled())
					.Select(
						u =>
							u.Filters.Where(v => v.Enabled)
					);

			FilteredItems = TotalItems.AndOrFilterBy(filterGroups).ToList();
		}

		public override void ApplySorter()
		{
			if (Sorters is null) return;

			IEnumerable<Tuple<ISorterRegistration<TItem>, bool>> sorters =
				Sorters
					.Where(u => u.Item1.Enabled != null)
					.OrderBy(u => u.Item2.GetSiblingIndex())
					.Select(
						u => 
							new Tuple<ISorterRegistration<TItem>, bool>(u.Item1,u.Item2)
					);

			Items = FilteredItems.SortBy(sorters).ToList();
		}
	}
}