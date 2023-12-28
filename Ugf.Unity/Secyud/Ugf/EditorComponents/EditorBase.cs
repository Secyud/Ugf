using UnityEngine;

namespace Secyud.Ugf.EditorComponents
{
    /// <summary>
    /// Editor 的本质是为了设置属性，所以需要绑定一个上下文并设置初值和空值
    /// </summary>
    public abstract class EditorBase<TProperty> : MonoBehaviour
    {
        public TProperty Property { get; private set; }

        public virtual void Bind(TProperty property)
        {
            Property = property;
            if (property is null)
            {
                ClearData();
            }
            else
            {
                InitData();
            }
        }

        protected virtual void InitData()
        {
        }

        protected virtual void ClearData()
        {
        }
    }
}