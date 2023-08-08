#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
	public sealed class FilterTriggerDescriptor<TItem> : FilterBaseDescriptor
	{
		public string Name { get; set; }

		public override string ShowName => Name;

		public List<FilterToggleDescriptor<TItem>> Filters { get; set; } = new();
	}
}