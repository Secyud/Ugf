#region

using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.TableComponents.FilterComponents;
using Secyud.Ugf.TableComponents.SorterComponents;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public static class RegistrationExtension
	{
		private static IEnumerable<TTarget> OrFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<FilterToggleDescriptor<TTarget>> filters)
		{
			return targets.Where(
				u =>
					filters.Any(v => v.Filter(u))
			);
		}

		public static IEnumerable<TTarget> AndFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<FilterToggleDescriptor<TTarget>> filters)
		{
			return targets.Where(u => filters.All(v => v.Filter(u)));
		}

		public static IEnumerable<TTarget> AndOrFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<IEnumerable<FilterToggleDescriptor<TTarget>>> filterLists)
		{
			return filterLists.Aggregate(
				targets,
				(current, filters) =>
					current.OrFilterBy(filters)
			);
		}

		/// <summary>
		/// </summary>
		/// <param name="targets"></param>
		/// <param name="sorters">bool: desc if true</param>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static IEnumerable<TTarget> SortBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<SorterToggleDescriptor<TTarget>> sorters)
		{
			return sorters.Reverse().Aggregate(
				targets,
				(current, sorter) =>
					sorter.Enabled == true
						? current.OrderByDescending(sorter.SortValue)
						: current.OrderBy(sorter.SortValue)
			);
		}
	}
}