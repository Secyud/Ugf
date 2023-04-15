#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class FunctionalTable : Table
    {
        public Transform FixedContent;
        public Transform SortableContent;
        public FilterGroup FilterGroupTemplate;
        public Sorter SorterTemplate;

        public List<FilterGroup> FilterGroups { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            FilterGroups = new();
        }
    }
}