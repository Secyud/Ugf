using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Unity.Prefabs
{
    public abstract class PrefabBase : MonoBehaviour
    {
        internal IDependencyProvider DependencyProvider;

        public T Get<T>() where T : class => DependencyProvider.Get<T>();
        
        public virtual void OnInitialize()
        {
        }
    }
}