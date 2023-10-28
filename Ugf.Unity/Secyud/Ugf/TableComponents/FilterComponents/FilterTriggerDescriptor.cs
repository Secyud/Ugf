#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
	public sealed class FilterTriggerDescriptor<TItem> : FilterBaseDescriptor
	{
		public List<FilterToggleDescriptor<TItem>> Filters { get; set; } = new();
	}
}