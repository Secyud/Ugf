#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public interface IHasSorters<in TItem>
	{
		public IEnumerable<ISorterRegistration<TItem>> Sorters { get; }
	}
}