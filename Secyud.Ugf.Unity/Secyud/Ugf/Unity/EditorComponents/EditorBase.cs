using UnityEngine;

namespace Secyud.Ugf.Unity.EditorComponents
{
    /// <summary>
    /// Editor 
    /// </summary>
    public abstract class EditorBase<TProperty> : MonoBehaviour
    {
        public TProperty Property { get; private set; }

        public virtual void Bind(TProperty property)
        {
            Property = property;
        }
    }
}