using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class BoolFieldInObject : FieldInObject
    {
        [SerializeField] protected Toggle BoolToggle;

        protected override void BindValue(object value)
        {
            BoolToggle.SetIsOnWithoutNotify((bool)value);
        }

        public void SetBoolean(bool b)
        {
            SetValue(b);
        }
    }
}