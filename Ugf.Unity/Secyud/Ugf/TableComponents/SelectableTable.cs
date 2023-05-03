#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class SelectableTable : FunctionalTable
    {
        public Transform ShowCellContent;
        
        public event Action EnsureAction;
        public event Action CancelAction;

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