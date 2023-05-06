#region

using System;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class SelectableTable : FunctionalTable
    {
        public Transform ShowCellContent;
        
        public event UnityAction EnsureAction;
        public event UnityAction CancelAction;

        public void Cancel()
        {
            CancelAction?.Invoke();
        }

        public void Ensure()
        {
            EnsureAction?.Invoke();
            CancelAction?.Invoke();
        }
    }
}