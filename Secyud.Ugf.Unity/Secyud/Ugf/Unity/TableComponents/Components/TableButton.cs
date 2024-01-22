using System.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.Components
{
    /// <summary>
    /// Used to call button groups for table.
    /// </summary>
    public class TableButton : MonoBehaviour
    {
        [SerializeField] private Table _table;
        [SerializeField] private Button _buttonTemplate;
        [SerializeField] private LayoutTrigger _content;
        [SerializeField] private RectTransform _window;

        private List<ITableButtonDescriptor> _buttons;

        private void Awake()
        {
            _buttons = new List<ITableButtonDescriptor>();
            if (!_window)
            {
                _window = _content.RectTransform;
            }
        }

        private void Start()
        {
            foreach (TableCell cell in _table.Content.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<RightClick>()
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
            _window.gameObject.SetActive(true);
            _content.ClearContent();
            foreach (ITableButtonDescriptor descriptor in _buttons)
            {
                if (descriptor.Visible(cell.CellObject))
                {
                    Button button = _buttonTemplate.Instantiate(_content.RectTransform);
                    descriptor.SetButton(button);
                    button.onClick.AddListener(CloseButtonWindow);
                }
            }
            
            _window.localPosition = 
                cell.transform.localPosition + 
                _table.Content.transform.position -
                _window.parent.position;
            _window.gameObject.SetActive(true);
            _content.Refresh();
        }

        private void CloseButtonWindow()
        {
            _window.gameObject.SetActive(false);
        }
    }
}