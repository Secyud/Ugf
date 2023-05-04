#region

using Secyud.Ugf.Layout;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class FunctionalTable : Table
    {
        public GridLayoutTrigger FixedContent;
        public GridLayoutTrigger SortableContent;
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