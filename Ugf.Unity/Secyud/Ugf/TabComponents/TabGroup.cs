﻿using System;
using System.Linq;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.LayoutComponents;
using UnityEngine;

namespace Secyud.Ugf.TabComponents
{
    public abstract class TabGroup<TService, TItem> : MonoBehaviour
        where TService : TabService<TService, TItem>
        where TItem : TabItem<TService, TItem>
    {
        [SerializeField] private LayoutGroupTrigger TabLabelContent;
        [SerializeField] private SLabelButton ButtonTemplate;

        protected TService Service;
        protected TItem CurrentTab;

        protected virtual void Start()
        {
            Service = U.Get<TService>();

            foreach ((string label, TItem item) in Service.RefreshItems)
            {
                SLabelButton button = Instantiate(ButtonTemplate, TabLabelContent.RectTransform);
                button.Text = U.T[label];
                button.Bind(() => SelectTab(item));
                item.GameObject.SetActive(false);
            }
            
            TabLabelContent.enabled = true;
            Service.Refresh();
            SelectTab(Service.RefreshItems.Values.First());
        }

        protected virtual void SelectTab(TItem tab)
        {
            if (CurrentTab?.GameObject)
                CurrentTab.GameObject.SetActive(false);
            CurrentTab = tab;
            if (CurrentTab?.GameObject)
                CurrentTab.GameObject.SetActive(true);
        }
    }
}