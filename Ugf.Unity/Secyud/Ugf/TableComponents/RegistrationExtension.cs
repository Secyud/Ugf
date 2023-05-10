#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public static class RegistrationExtension
	{
		private static IEnumerable<TTarget> OrFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<FilterRegistration<TTarget>> filters)
		{
			return targets.Where(
				u =>
					filters.Any(v => v.Filter(u))
			);
		}

		public static IEnumerable<TTarget> AndFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<FilterRegistration<TTarget>> filters)
		{
			return targets.Where(u => filters.All(v => v.Filter(u)));
		}

		public static IEnumerable<TTarget> AndOrFilterBy<TTarget>(this IEnumerable<TTarget> targets,
			IEnumerable<IEnumerable<FilterRegistration<TTarget>>> filterLists)
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
			IEnumerable<Pair<ISorterRegistration<TTarget>, bool>> sorters)
		{
			return sorters.Reverse().Aggregate(
				targets,
				(current, sorter) =>
					sorter.Second
						? current.OrderByDescending(sorter.First.SortValue)
						: current.OrderBy(sorter.First.SortValue)
			);
		}
	}
}