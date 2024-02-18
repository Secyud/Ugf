using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] protected TabPanel[] Panels;
        protected TabPanel CurrentTab;

        protected virtual void Awake()
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
            if (CurrentTab)
            {
                CurrentTab.transform.localPosition = new Vector3(0, 65536, 0);
                CurrentTab.OnHiding();
            }

            CurrentTab = tab;

            if (CurrentTab)
            {
                CurrentTab.transform.localPosition = Vector3.zero;
                CurrentTab.OnShowing();
            }
        }

        protected virtual void OnDestroy()
        {
            if (CurrentTab)
            {
                CurrentTab.OnHiding();
            }
        }
    }
}