using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableSource : TableSource
    {
        public event Func<IEnumerable<object>> SourceGetter;

        public IEnumerable<object> SourceData { get; protected set; }

        public override void Apply()
        {
            SourceData = SourceGetter?.Invoke();
        }
    }
}