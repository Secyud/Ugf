using UnityEngine;

namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public class SingleSelect:TableComponentBase<SingleSelect,SingleSelectDelegate>
    {
        public override string Name => nameof(SingleSelect);
        
        [SerializeField] private TableCell ShowCell;
        [SerializeField] private GameObject GameObject;

        private void Awake()
        {
            if (!GameObject)
            {
                GameObject = gameObject;
            }
        }

        public TableCell Cell => ShowCell;

        public void OnEnsure()
        {
            Delegate.OnEnsure();
            OnCancel();
        }

        public void OnCancel()
        {
            Destroy(GameObject);
        }
    }
}