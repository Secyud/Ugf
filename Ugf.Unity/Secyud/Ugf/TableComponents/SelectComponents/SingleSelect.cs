using UnityEngine;

namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public class SingleSelect:TableComponentBase<SingleSelect,SingleSelectDelegate>
    {
        public override string Name => nameof(SingleSelect);
        
        [SerializeField] private TableCell ShowCell;

        public TableCell Cell => ShowCell;

        public void OnEnsure()
        {
            Delegate.OnEnsure();
        }
    }
}