using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.UserInterface
{
    public abstract class UiControllerBase : ISingleton
    {
        internal UiDescriptor UiDescriptor { get; set; }
        
        public string Name { get; }

        protected GameObject PanelObject => UiDescriptor.Instance;

        protected TComponent GetComponent<TComponent>()
            where TComponent : Component
            => UiDescriptor.Instance.GetComponent<TComponent>();


        public virtual void OnInitialize()
        {
        }

        public virtual void OnPause()
        {
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnExit()
        {
        }
        
        protected UiControllerBase()
        {
            var className = GetType().Name;
            Name = className.EndsWith("Controller") ? className[..^"Controller".Length] : className;
        }
    }
}