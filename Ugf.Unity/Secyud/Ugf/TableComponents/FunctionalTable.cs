#region

using Secyud.Ugf.Layout;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public class FunctionalTable : Table
	{
		public LayoutGroupTrigger FixedContent;
		public LayoutGroupTrigger SortableContent;
		public FilterGroup FilterGroupTemplate;
		public Sorter SorterTemplate;

		public List<FilterGroup> FilterGroups { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			FilterGroups = new List<FilterGroup>();
		}
	}
}