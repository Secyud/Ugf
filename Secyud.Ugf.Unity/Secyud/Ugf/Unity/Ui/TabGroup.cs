using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabGroup : MonoBehaviour
    {
        private TabPanel _currentTab;

        internal List<TabPanel> Panels;

        private void Awake()
        {
            Panels = new List<TabPanel>();
        }

        protected virtual void Start()
        {
            SelectTab(Panels[0]);
        }

        public void SelectTab(TabPanel tab)
        {
            if (_currentTab)
            {
                _currentTab.transform.localPosition += new Vector3(0, 65536, 0);
            }
            
            _currentTab = tab;
            
            if (_currentTab)
            {
                _currentTab.transform.localPosition = Vector3.zero;
            }
        }
    }
}