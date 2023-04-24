#region

using System;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class SelectableTable : FunctionalTable
    {
        public event Action EnsureAction;
        public event Action CancelAction;

        private void Cancel()
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