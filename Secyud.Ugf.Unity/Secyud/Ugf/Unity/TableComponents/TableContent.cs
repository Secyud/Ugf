using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 视图内容，一般使用默认
    /// </summary>
    [RequireComponent(typeof(Table))]
    public class TableContent : MonoBehaviour
    {
        [field: SerializeField] public RectTransform Content { get; set; }
        public TableCell[] Cells { get; private set; }

        protected TablePager TablePager;
        public event Action<TableCell> SetCellEvent;

        protected virtual void Awake()
        {
            TablePager = GetComponent<TablePager>();

            List<TableCell> cells = ListPool<TableCell>.Get();
            
            for (int i = 0; i < Content.childCount; i++)
            {
                if (Content.GetChild(i).TryGetComponent(out TableCell cell))
                {
                    cells.Add(cell);
                }
            }
            
            Cells = cells.ToArray();
            
            ListPool<TableCell>.Release(cells);
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