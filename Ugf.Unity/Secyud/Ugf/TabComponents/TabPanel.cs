using UnityEngine;

namespace Secyud.Ugf.TabComponents
{
    public abstract class TabPanel : MonoBehaviour
    {
        public virtual string Name => GetType().Name;
        protected abstract TabService Service { get; }
        
        protected virtual void Awake()
        {
            Service.AddTab(this);
        }

        public abstract void RefreshTab();
    }
}