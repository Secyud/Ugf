using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public class Table : MonoBehaviour
    {
        private TableSource _source;
        private TableFilter _filter;
        private TableSorter _sorter;
        private TablePager _pager;
        private TableContent _content;

        protected int State { get; set; }

        protected virtual void Awake()
        {
            _source = GetComponent<TableSource>();
            _filter = GetComponent<TableFilter>();
            _sorter = GetComponent<TableSorter>();
            _pager = GetComponent<TablePager>();
            _content = GetComponent<TableContent>();
        }

        public void Refresh(int state)
        {
            State = Math.Max(state, State);
            enabled = true;
        }

        protected virtual void LateUpdate()
        {
            if (State > 3) _source.Apply();
            if (State > 2) _filter.Apply();
            if (State > 1) _sorter.Apply();
            if (State > 0) _pager.Apply();
            if (State > -1) _content.Apply();
            State = -1;
            enabled = false;
        }
    }
}