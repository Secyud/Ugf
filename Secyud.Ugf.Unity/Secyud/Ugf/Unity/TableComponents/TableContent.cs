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

        protected TablePager TablePager;
        public event Action<TableCell> SetCellEvent;

        protected virtual void Awake()
        {
            TablePager = GetComponent<TablePager>();

            TableCell prefab = Cells[0];
            Transform content = prefab.transform.parent;

            for (int i = 0; i < Cells.Length; i++)
            {
                if (!Cells[i])
                    Cells[i] = prefab.Instantiate(content);
            }
        }

        public virtual void Apply(bool soft)
        {
            IList<object> list = TablePager.PagedData;
            for (int i = 0; i < Cells.Length; i++)
            {
                var cellObject = i < list.Count ? list[i] : null;
                if (cellObject == Cells[i].CellObject && soft) continue;
                Cells[i].SetObject(cellObject);
                SetCellEvent?.Invoke(Cells[i]);
            }
        }
    }
}