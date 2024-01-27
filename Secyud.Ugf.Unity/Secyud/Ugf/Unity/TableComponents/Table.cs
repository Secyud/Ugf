using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public class Table : MonoBehaviour
    {
        [field: SerializeField] public TableSource Source { get; private set; }
        [field: SerializeField] public TableDataOperator Filter { get; private set; }
        [field: SerializeField] public TableDataOperator Sorter { get; private set; }
        [field: SerializeField] public TablePager Pager { get; private set; }
        [field: SerializeField] public TableContent Content { get; private set; }

        protected int State;

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