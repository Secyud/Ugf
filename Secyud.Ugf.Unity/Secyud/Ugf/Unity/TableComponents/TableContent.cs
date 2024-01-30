using System;
using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 视图内容，一般使用默认
    /// </summary>
    [RequireComponent(typeof(Table))]
    public class TableContent : MonoBehaviour
    {
        [field: SerializeField] public TableCell[] Cells { get; private set; }
        public Table Table { get; private set; }
        public event Action<TableCell> SetCellEvent;

        protected virtual void Awake()
        {
            Table = GetComponent<Table>();
        }

        public virtual void Apply()
        {
            IList<object> list = Table.Pager.PagedData;
            for (int i = 0; i < Cells.Length; i++)
            {
                var cellObject = i < list.Count ? list[i] : null;
                if (cellObject == Cells[i].CellObject) continue;
                Cells[i].SetObject(cellObject);
                SetCellEvent?.Invoke(Cells[i]);
            }
        }
    }
}