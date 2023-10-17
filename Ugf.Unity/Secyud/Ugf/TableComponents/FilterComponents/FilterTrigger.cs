#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public class FilterTrigger : FilterBase<FilterTrigger>, IFilterGroup<FilterToggle>
    {
        [SerializeField] private FilterToggle FilterTemplate;
        [SerializeField] private FilterPopup PopupTemplate;

        public List<ICanBeEnabled> Registrations { get; } = new();
        public List<FilterToggle> Filters { get; } = new();

        public Filter Content { get; private set; }

        public void RefreshTable()
        {
            Content.RefreshTable();
        }

        public void OnClick()
        {
            IsDropped = !IsDropped;
        }

        public bool IsDropped
        {
            get => Content.DroppedFilter == this && FilterPopup.PopupExist;
            set
            {
                if (value)
                    OpenPopup();
                else
                    ClosePopup();
            }
        }

        private void OpenPopup()
        {
            if (Content.DroppedFilter)
                Content.DroppedFilter.IsDropped = false;

            Content.DroppedFilter = this;

            Filters.Clear();

            RectTransform content = PopupTemplate.Open();

            foreach (ICanBeEnabled registration in Registrations)
                Filters.Add(FilterTemplate.Create(content, this, registration));
        }

        private void ClosePopup()
        {
            PopupTemplate.Close();
            Content.DroppedFilter = null;
            Filters.Clear();
        }

        public FilterTrigger Create(Transform parent, Filter filter, ICanBeEnabled canBeEnabled)
        {
            FilterTrigger ret = Instantiate(this, parent);

            ret.Content = filter;
            ret.SetFilter(filter, canBeEnabled);

            return ret;
        }
    }
}