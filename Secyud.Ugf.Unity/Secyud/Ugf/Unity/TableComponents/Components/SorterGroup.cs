using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.TableComponents.Components
{
    /// <summary>
    /// <para>
    /// The ui component of sorter，it controls the
    /// state of sorter, use sub class of
    /// <see cref="TableSorter"/> to manage sort way.
    /// </para>
    /// <para>
    /// Local usage see
    /// <see cref="TableExtension.InitLocalSorterGroup{TSorter}"/>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(LayoutTrigger))]
    public class SorterGroup : MonoBehaviour
    {
        [SerializeField] private SorterToggle _toggleTemplate;
        [SerializeField] private TableDataOperator _sorterFunction;
        [SerializeField] private RectTransform _sorterLine;
        private List<SorterToggle> _sorters;
        private LayoutTrigger _layoutTrigger;

        private float _width;
        private Vector2 _deltaRecord;
        private SorterToggle _sortToggle;
        private List<RectTransform> _otherToggles;

        private void Awake()
        {
            _width = _toggleTemplate.RectTransform.rect.width / 2;
            _layoutTrigger = GetComponent<LayoutTrigger>();
            _sorters = new List<SorterToggle>();
        }

        public void Initialize<TSorter>(
            IEnumerable<TSorter> sorters)
            where TSorter:ITableSorterDescriptor
        {
            foreach (TSorter sorter in sorters)
            {
                _sorters.Add(_toggleTemplate
                    .Instantiate(transform)
                    .Initialize(this, sorter));
            }
        }

        public IList<ITableSorterDescriptor> GetWorkedSorters()
        {
            return _sorters
                .Where(toggle => toggle.State != null)
                .OrderBy(u => u.transform.GetSiblingIndex())
                .Select(toggle => toggle.Sorter)
                .ToList();
        }

        public void Refresh()
        {
            _sorterFunction.Table.Refresh(2);
        }

        public void BeginToggleDrag(SorterToggle toggle, PointerEventData eventData)
        {
            toggle.RectTransform.SetParent(U.Canvas.gameObject.transform);
            _deltaRecord = toggle.RectTransform.anchoredPosition - eventData.position;
            _sortToggle = toggle;

            _otherToggles.Clear();
            Transform layout = transform;
            for (int i = 0; i < layout.childCount; i++)
            {
                Transform child = layout.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    _otherToggles.Add(child as RectTransform);
                }
            }

            _layoutTrigger.Refresh(int.MaxValue);
            _sorterLine.gameObject.SetActive(true);
        }

        public void ToggleDrag(PointerEventData eventData)
        {
            _sortToggle.RectTransform.anchoredPosition =
                eventData.position + _deltaRecord;

            float compareValue = _sorterLine.anchoredPosition.x - _width;
            RectTransform target = _otherToggles.FirstOrDefault(
                u => u.anchoredPosition.x - compareValue > 0);

            if (target)
            {
                _sorterLine.SetSiblingIndex(target.GetSiblingIndex());
            }
            else
            {
                _sorterLine.SetSiblingIndex(transform.childCount - 1);
            }
        }

        public void EndToggleDrag(PointerEventData eventData)
        {
            if (_sortToggle)
            {
                _sortToggle.RectTransform.SetParent(transform);
                int index = _sorterLine.GetSiblingIndex();
                _sortToggle.transform.SetSiblingIndex(index);
            }

            _layoutTrigger.Refresh(0);
            _sorterLine.gameObject.SetActive(false);
        }
    }
}