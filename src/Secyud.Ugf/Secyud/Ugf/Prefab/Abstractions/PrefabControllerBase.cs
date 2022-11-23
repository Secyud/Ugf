using System;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public abstract class PrefabControllerBase : ISingleton
    {
        protected PrefabControllerBase()
        {
            var className = GetType().Name;
            Name = className.EndsWith("Controller") ? className[..^"Controller".Length] : className;
        }

        internal PrefabDescriptor PrefabDescriptor { get; set; }

        public string Name { get; protected set; }

        public virtual Type ParentType => null;

        internal Func<PrefabControllerBase, PrefabControllerBase> ParentFactory { get; set; }

        public PrefabControllerBase LogicParent => ParentFactory(this);
        
        public GameObject Parent => PrefabDescriptor.Instance.GetComponentInParent<Transform>().parent.gameObject;

        public GameObject GameObject => PrefabDescriptor.Instance;

        protected TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            return PrefabDescriptor.Instance.GetComponent<TComponent>();
        }


        public virtual void OnInitialize()
        {
        }

        public virtual void OnShutDown()
        {
        }
    }
}