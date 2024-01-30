using Secyud.Ugf.Abstraction;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    [RequireComponent(typeof(Table))]
    public class HoverFloating : MonoBehaviour
    {
        [SerializeField] private LayoutTrigger _floating;
        private TableContent _tableContent;

        protected virtual void Awake()
        {
            _tableContent = GetComponent<TableContent>();
            _floating.gameObject.GetOrAddComponent<PointerExit>()
                .OnPointExit.AddListener(CloseFloatingWindow);
        }

        protected virtual void Start()
        {
            foreach (TableCell cell in _tableContent.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<Hoverable>()
                    .OnHover
                    .AddListener(() => OpenFloatingWindow(cell));
            }
        }

        private void OpenFloatingWindow(TableCell cell)
        {
            if (cell.CellObject is not IHasContent hasContent)
                return;

            _floating.ClearContent();
            hasContent.SetContent(_floating.RectTransform);
            _floating.ActivateFloating(cell.transform);
        }

        private void CloseFloatingWindow()
        {
            _floating.gameObject.SetActive(false);
        }
    }
}