using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Prefabs
{
    public abstract class PrefabBase : MonoBehaviour
    {
        internal IDependencyProvider DependencyProvider;
        
        public T GetDependency<T>() where T : class => DependencyProvider.GetDependency<T>();
        
        public virtual void OnInitialize()
        {
        }
    }
}