﻿using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.LayoutComponents;
using UnityEngine;

namespace Secyud.Ugf.TabComponents
{
    public abstract class TabGroup : MonoBehaviour
    {
        [SerializeField] private LayoutGroupTrigger TabLabelContent;
        [SerializeField] private SLabelButton ButtonTemplate;

        protected abstract TabService Service { get; }
        protected TabPanel CurrentTab;

        protected virtual void Awake()
        {
            Service.TabGroup = this;
        }

        protected virtual void Start()
        {
            RefreshTabGroup();
        }

        public void RefreshTabGroup()
        {
            RectTransform rectTransform = TabLabelContent.PrepareLayout();

            TabPanel last = null;
            foreach (TabPanel tab in Service.Tabs)
            {
                SLabelButton button = Instantiate(ButtonTemplate, rectTransform);
                button.Text = U.T[tab.Name];
                button.Bind(() => SelectTab(tab));
                tab.gameObject.SetActive(false);
                last = tab;
            }
            SelectTab(last);
        }

        protected virtual void SelectTab(TabPanel tab)
        {
            if (CurrentTab)
                CurrentTab.gameObject.SetActive(false);
            CurrentTab = tab;
            if (CurrentTab)
            {
                CurrentTab.gameObject.SetActive(true);
                CurrentTab.RefreshTab();
            }
        }

        public void RefreshCurrentTab()
        {
            if (CurrentTab)
            {
                CurrentTab.RefreshTab();
            }
        }
    }
}