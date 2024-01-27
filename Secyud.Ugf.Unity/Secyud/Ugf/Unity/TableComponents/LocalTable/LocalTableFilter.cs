using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableFilter : TableDataOperator
    {
        public event Func<IEnumerable<object>, IEnumerable<object>> FilterEvent;

        public IEnumerable<object> FilteredData { get; protected set; }

        public override void Apply()
        {
            if (Table.Source is LocalTableSource
                {
                    SourceData: not null
                } localSource)
            {
                FilteredData =
                    (FilterEvent?.Invoke(localSource.SourceData)
                     ?? localSource.SourceData).ToList();
            }
        }
    }
}