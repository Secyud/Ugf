using System;
using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 视图内容，一般使用默认
    /// </summary>
    public class TableContent : MonoBehaviour
    {
        [field:SerializeField]public TableCell[] Cells { get; private set; }
        public Table Table { get; internal set; }
        public event Action<TableCell> SetCellEvent; 
        
        public virtual void Apply()
        {
            IList<object> list = Table.Pager.PagedData;
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].SetObject(i < list.Count ? list[i] : null);
                SetCellEvent?.Invoke(Cells[i]);
            }
        }
    }
}