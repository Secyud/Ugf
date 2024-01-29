using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableSource : TableSource
    {
        private Func<IEnumerable<object>> _sourceGetter;

        public IEnumerable<object> SourceData { get; protected set; }

        public void SetSource(Func<IEnumerable<object>> getter)
        {
            _sourceGetter = getter;
        }
        
        public override void Apply()
        {
            SourceData = _sourceGetter?.Invoke();
        }
    }
}