using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class GuidFieldInObject : FieldInObject
    {
        [SerializeField] protected TMP_InputField GuidInput;
        [SerializeField] protected TextMeshProUGUI ClassNameTip;
        [SerializeField] protected Image InvalidIcon;

        protected override void BindValue(object value)
        {
            Guid id = (Guid)value;
            GuidInput.SetTextWithoutNotify(value.ToString());
            GuidInput.interactable = Field.ShowType == SShowType.Special;
            ClassNameTip.text = TypeManager.Instance[id]?.Type.Name;
        }

        public void SetGuid(string str)
        {
            InvalidIcon.enabled =
                !Guid.TryParse(str, out Guid id);
            if (!InvalidIcon.enabled) SetValue(id);
        }

        public void OpenClassSelect()
        {
            var attr = Field.Info.GetCustomAttribute<TypeLimitAttribute>();
            UnityDataManagerService.OpenClassSelectPanel(
                attr?.LimitType, SetClass);
            return;

            void SetClass(Type classType)
            {
                SetValue(classType.GUID);
                BindValue(classType.GUID);
            }
        }
    }
}