using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 视图内容，一般使用默认
    /// </summary>
    public class TableContent : MonoBehaviour
    {
        [SerializeField] private TableCell _cellTemplate;
        [SerializeField] private int _maxShowCount;
        public TableCell[] Cells { get; private set; }
        public Table Table { get; internal set; }

        protected virtual void Awake()
        {
            Cells = new TableCell[_maxShowCount];
            for (int i = 0; i < _maxShowCount; i++)
            {
                Cells[i] = _cellTemplate.Instantiate(transform);
            }
        }

        public virtual void Apply()
        {
            IList<object> list = Table.Pager.PagedData;
            for (int i = 0; i < _maxShowCount; i++)
            {
                Cells[i].SetObject(i < list.Count ? list[i] : null);
            }
        }
    }
}