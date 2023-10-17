using System;

namespace Secyud.Ugf.ObserverComponents
{
    public class ObserverItem
    {
        public string Name { get; }
        public Action RefreshAction { get;  }
        
        public virtual bool Valid => true;
        
        public ObserverItem(string name,Action refreshAction)
        {
            Name = name;
            RefreshAction = refreshAction;
        }

        public void Refresh()
        {
            RefreshAction?.Invoke();
        }
    }
}