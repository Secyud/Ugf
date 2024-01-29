using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableFilter : TableDataOperator
    {
        public readonly List<Func<IEnumerable<object>, IEnumerable<object>>> FilterEvent = new();

        public List<object> FilteredData { get; } = new();

        public override void Apply()
        {
            if (Table.Source is LocalTableSource
                {
                    SourceData: not null
                } localSource)
            {
                IEnumerable<object> source = localSource.SourceData;
                foreach (var func in FilterEvent)
                {
                    source = func.Invoke(source);
                }

                FilteredData.Clear();
                FilteredData.AddRange(source);
            }
        }
    }
}