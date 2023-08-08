using UnityEngine;

namespace Secyud.Ugf.EditorComponents
{
    public abstract class EditorBase<TProperty> :MonoBehaviour
    {
        public TProperty Property;
        
        public virtual void Bind(TProperty property)
        {
            Property = property;
            if (property is null)
                ClearUi();
            else
                InitData();
        }

        protected virtual void InitData()
        {
            
        }
        protected virtual void ClearUi()
        {
            
        }
    }
}