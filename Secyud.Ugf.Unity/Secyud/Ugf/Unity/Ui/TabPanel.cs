using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabPanel : MonoBehaviour
    {
        public TabGroup Group { get;internal set; }

        public void SelectThisTab()
        {
            Group.SelectTab(this);
            Refresh();
        }

        protected virtual void Refresh()
        {
        }
    }
}