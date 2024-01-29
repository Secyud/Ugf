using UnityEngine;

namespace Secyud.Ugf.Unity.EditorComponents
{
    /// <summary>
    /// Editor simplify the bind of component.
    /// Maybe more functions.
    /// </summary>
    public abstract class EditorBase<TProperty> : MonoBehaviour
    {
        protected TProperty Property { get; private set; }

        public virtual void Bind(TProperty property)
        {
            Property = property;
        }

        public void Refresh()
        {
            Bind(Property);
        }
    }
}