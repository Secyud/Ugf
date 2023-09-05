using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.ObserverComponents
{
    [RegistryDisabled]
    public class ObservedService: IRegistry
    {
        public List<ObserverItem> Observed { get; } = new();
        
        public virtual void Refresh()
        {
            int count = Observed.Count;
            for (int i = 0; i < count;)
            {
                ObserverItem observerItem = Observed[i];
                if (observerItem.Valid)
                {
                    observerItem.Refresh();
                    i++;
                }
                else
                {
                    Observed.RemoveAt(i);
                    count--;
                }
            }
        }

        public void AddObserverObject(string name, Action action,GameObject gameObject)
        {
            AddObserver(new ObserverObject(name, action, gameObject));
        }
        
        public void AddObserverItem(string name, Action action)
        {
            AddObserver(new ObserverItem(name, action));
        }

        private void AddObserver(ObserverItem observer)
        {
            for (int i = 0; i < Observed.Count; i++)
            {
                if (Observed[i].Name != observer.Name)
                    continue;
                Observed[i] = observer;
                return;
            }
            
            Observed.Add(observer);
        }
    }
}