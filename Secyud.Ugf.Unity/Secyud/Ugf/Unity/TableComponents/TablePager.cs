using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public abstract class TablePager:MonoBehaviour
    {
        public Table Table { get; internal set; }
        public IList<object> PagedData { get; protected set; }
        public abstract void Apply();
    }
}