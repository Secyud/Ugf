using System.Linq;
using Secyud.Ugf.BasicComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public abstract class FilterBase<TSelf> : SToggle where TSelf : FilterBase<TSelf>
    {
        [SerializeField] private SText Name;
        public IFilterGroup<TSelf> Parent { get; private set; }

        public override void OnPointerDown(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    base.OnPointerDown(eventData);
                    Refresh();
                    break;
                case PointerEventData.InputButton.Right:
                {
                    if (IsInteractable() && navigation.mode != Navigation.Mode.None && EventSystem.current != null)
                        EventSystem.current.SetSelectedGameObject(gameObject, eventData);

                    DoStateTransition(SelectionState.Pressed, true);

                    bool value = Parent.Filters.All(u => (this == u) ^ u.isOn);

                    foreach (TSelf filter in Parent.Filters)
                    {
                        filter.isOn = (this == filter) ^ !value;
                        filter.Refresh();
                    }

                    break;
                }
                case PointerEventData.InputButton.Middle:
                default: break;
            }
        }

        private void Refresh()
        {
            Parent.RefreshTable();
        }

        protected virtual void SetFilter(IFilterGroup<TSelf> parent, ICanBeEnabled canBeEnabled)
        {
            Parent = parent;
            SetIsOnWithoutNotify(canBeEnabled.GetEnabled());
            Bind(canBeEnabled.SetEnabled);
            Name.text = U.T[canBeEnabled.ShowName];
        }
    }
}