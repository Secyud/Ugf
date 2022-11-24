using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public abstract class PrefabControllerBase : ITransient
    {
        protected PrefabControllerBase()
        {
            var className = GetType().Name;
            Name = className.EndsWith("Controller") ? className[..^"Controller".Length] : className;
        }

        internal PrefabDescriptor PrefabDescriptor { get; set; }

        public string Name { get; protected set; }
        
        public GameObject ParentObject => PrefabDescriptor.Instance.transform.parent.gameObject;

        public GameObject GameObject => PrefabDescriptor.Instance;

        protected TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            return PrefabDescriptor.Instance.GetComponent<TComponent>();
        }
        
        protected TComponent AddComponent<TComponent>()
            where TComponent : Component
        {
            return PrefabDescriptor.Instance.AddComponent<TComponent>();
        }


        public virtual void OnInitialize()
        {
        }

        public virtual void OnShutDown()
        {
        }
    }
}