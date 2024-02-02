using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public abstract class DataFieldBase : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI LabelText;

        protected abstract DataFieldBase ParentField { get; }

        public virtual void RefreshContent(int level)
        {
            ParentField.RefreshContent(level + 1);
        }

        protected abstract void BindValue(object value);
        protected abstract void SetValue(object value);
        protected abstract object GetValue();
    }
}