using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// <para>
    /// The inner component of <see cref="SorterGroup"/>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SorterToggle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private LeftClick _stateChangeClick;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;
        [SerializeField] private SorterGroup _group;
        private CanvasGroup _canvasGroup;
        public ITableSorterDescriptor Sorter { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
            _stateChangeClick.OnClick.AddListener(OnStateChange);
        }

        public bool? State
        {
            get => Sorter.State;
            set
            {
                SetStateWithoutNotification(value);
                _group.Refresh();
            }
        }

        private void OnStateChange()
        {
            State = Sorter.State switch
            {
                true => false, false => null, null => true
            };
        }

        public SorterToggle Initialize(SorterGroup group,
            ITableSorterDescriptor sorter)
        {
            _text.text = U.T[sorter.Name];
            _group = group;
            SetStateWithoutNotification(sorter.State);
            Sorter = sorter;
            return this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            _canvasGroup.blocksRaycasts = false;
            _group.BeginToggleDrag(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            _group.ToggleDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            _group.EndToggleDrag(eventData);
        }

        public void SetStateWithoutNotification(bool? state)
        {
            Sorter.State = state;
            _image.transform.eulerAngles = new Vector3(0, 0,
                state switch
                {
                    true => 90, false => -90, null => 0
                });
        }
    }
}