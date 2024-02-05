using System;
using System.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// Used to call button groups for table.
    /// </summary>
    public class TableButton : MonoBehaviour
    {
        [SerializeField] private Button _buttonTemplate;
        [SerializeField] private LayoutTrigger _floating;

        private List<ITableButtonDescriptor> _buttons;

        private void Awake()
        {
            _buttons = new List<ITableButtonDescriptor>();
        }

        private void Start()
        {
            TableContent tableContent = GetComponent<TableContent>();
            foreach (TableCell cell in tableContent.Cells)
            {
                cell.GetOrAddComponent<RightClick>()
                    .OnClick
                    .AddListener(() => OpenButtonWindow(cell));
            }
        }

        public void Initialize(IEnumerable<ITableButtonDescriptor> buttonDescriptors)
        {
            foreach (ITableButtonDescriptor descriptor in buttonDescriptors)
            {
                _buttons.AddIfNotContains(descriptor);
            }
        }

        private void OpenButtonWindow(TableCell cell)
        {
            _floating.ClearContent();
            foreach (ITableButtonDescriptor descriptor in _buttons)
            {
                if (descriptor.Visible(cell.CellObject))
                {
                    Button button = _buttonTemplate.Instantiate(_floating.RectTransform);
                    descriptor.SetButton(button,cell.CellObject);
                    button.onClick.AddListener(CloseButtonWindow);
                }
            }
            _floating.ActivateFloating(cell.transform as RectTransform);
        }

        private void CloseButtonWindow()
        {
            _floating.gameObject.SetActive(false);
        }
    }
}