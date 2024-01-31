using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据分页方式，实现此类以自定义分页方式。
    /// 可结合<see cref="TableContent"/>
    /// 使用。
    /// </summary>
    [RequireComponent(typeof(Table))]
    public abstract class TablePager:MonoBehaviour
    {
        public List<object> PagedData { get; } = new();
        protected TableSorter Sorter;

        protected virtual void Awake()
        {
            Sorter = GetComponent<TableSorter>();
        }
        public abstract void Apply();
    }
}