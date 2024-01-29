using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabGroup : MonoBehaviour
    {
        private TabPanel _currentTab;

        [SerializeField]protected TabPanel[] Panels;

        private void Awake()
        {
            foreach (TabPanel panel in Panels)
            {
                panel.Group = this;
            }
        }

        public void InitTabGroup()
        {
            SelectTab(Panels[0]);
        }

        public virtual void SelectTab(TabPanel tab)
        {
            if (_currentTab)
            {
                _currentTab.transform.localPosition +=
                    new Vector3(0, 65536, 0);
            }

            _currentTab = tab;

            if (_currentTab)
            {
                _currentTab.transform.localPosition = Vector3.zero;
            }
        }
    }
}