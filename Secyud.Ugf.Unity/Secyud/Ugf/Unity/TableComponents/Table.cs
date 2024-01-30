using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public class Table : MonoBehaviour
    {
        public TableSource Source { get; private set; }
        public TableFilter Filter { get; private set; }
        public TableSorter Sorter { get; private set; }
        public TablePager Pager { get; private set; }
        public TableContent Content { get; private set; }

        protected int State { get; set; }

        protected virtual void Awake()
        {
            Source = GetComponent<TableSource>();
            Filter = GetComponent<TableFilter>();
            Sorter = GetComponent<TableSorter>();
            Pager = GetComponent<TablePager>();
            Content = GetComponent<TableContent>();
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