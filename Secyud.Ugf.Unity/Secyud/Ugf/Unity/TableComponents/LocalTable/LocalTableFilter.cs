using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableFilter : TableDataOperator
    {
        public readonly List<Func<IEnumerable<object>, IEnumerable<object>>> FilterEvent = new();

        public IEnumerable<object> FilteredData { get; protected set; }

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

                FilteredData = source.ToList();
            }
        }
    }
}