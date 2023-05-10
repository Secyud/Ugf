#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public interface IHasFilterGroups<TItem>
	{
		public IEnumerable<FilterRegistrationGroup<TItem>> FilterGroups { get; }
	}
}