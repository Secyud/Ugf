using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabPanel : MonoBehaviour
    {
        [SerializeField] private TabGroup _group;

        protected virtual void Awake()
        {
            _group.Panels.Add(this);
        }

        public void SelectThisTab()
        {
            _group.SelectTab(this);
            Refresh();
        }

        protected virtual void Refresh()
        {
        }
    }
}