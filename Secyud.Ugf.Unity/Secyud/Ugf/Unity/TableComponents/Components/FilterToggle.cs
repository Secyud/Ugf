using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.Components
{
    /// <summary>
    /// <para>
    /// The inner component of <see cref="FilterGroup"/>
    /// </para>
    /// </summary>
    public class FilterToggle : MonoBehaviour
    {
        [SerializeField] private LeftClick _leftClick;
        [SerializeField] private RightClick _rightClick;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private FilterGroup _group;
        public ITableFilterDescriptor Filter { get; private set; }

        private void Awake()
        {
            _leftClick.OnClick.AddListener(OnLeftClick);
            _rightClick.OnClick.AddListener(OnRightClick);
        }

        public FilterToggle Initialize(FilterGroup group,
            ITableFilterDescriptor filter)
        {
            _text.text = U.T[filter.Name];
            _group = group;
            _toggle.SetIsOnWithoutNotify(filter.State);
            Filter = filter;
            return this;
        }

        public bool IsOn
        {
            get => Filter.State;
            set
            {
                Filter.State = value;
                _toggle.SetIsOnWithoutNotify(value);
                _group.Apply();
            }
        }

        private void OnLeftClick()
        {
            IsOn = !IsOn;
        }

        private void OnRightClick()
        {
            if (_group)
            {
                _group.SetToggle(this);
            }
        }
    }
}