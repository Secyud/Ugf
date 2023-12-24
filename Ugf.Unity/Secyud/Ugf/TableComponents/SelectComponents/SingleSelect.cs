using UnityEngine;
using UnityEngine.Serialization;

namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public class SingleSelect : TableComponentBase<SingleSelect, SingleSelectDelegate>
    {
        public override string Name => nameof(SingleSelect);

        [SerializeField] private TableCell ShowCell;
        [SerializeField] private GameObject CancelGameObject;

        private void Awake()
        {
            if (!CancelGameObject)
            {
                CancelGameObject = gameObject;
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
            if (CancelGameObject)
            {
                Destroy(CancelGameObject);
            }
        }
    }
}