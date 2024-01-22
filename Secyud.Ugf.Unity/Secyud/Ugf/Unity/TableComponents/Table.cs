using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private TableSource _source;
        [SerializeField] private TableFilter _filter;
        [SerializeField] private TableSorter _sorter;
        [SerializeField] private TablePager _pager;
        [SerializeField] private TableContent _content;

        protected int State;
        public TableSource Source => _source;
        public TableFilter Filter => _filter;
        public TableSorter Sorter => _sorter;
        public TablePager Pager => _pager;
        public TableContent Content => _content;

        protected virtual void Awake()
        {
            Source.Table = this;
            Filter.Table = this;
            Sorter.Table = this;
            Pager.Table = this;
            Content.Table = this;
        }


        public void Refresh(int state)
        {
            State = Math.Max(state, State);
            enabled = true;
        }

        protected virtual void LateUpdate()
        {
            if (State > 3) Source.Apply();
            if (State > 2) Filter.Apply();
            if (State > 1) Sorter.Apply();
            if (State > 0) Pager.Apply();
            if (State > -1) Content.Apply();
            State = -1;
            enabled = false;
        }
    }
}