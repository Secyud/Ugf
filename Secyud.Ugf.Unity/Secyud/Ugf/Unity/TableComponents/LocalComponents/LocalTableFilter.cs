using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.TableComponents.LocalComponents
{
    public class LocalTableFilter : TableFilter
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