using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class GuidField : DataField
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _className;
        [SerializeField] private Image _wrong;

        public override void Bind(object parent, SAttribute sAttribute)
        {
            base.Bind(parent, sAttribute);
            Guid id = (Guid)sAttribute.GetValue(parent);
            _inputField.SetTextWithoutNotify(id.ToString());
            _inputField.interactable = sAttribute.ShowType == SShowType.Special;
            _className.text = TypeManager.Instance[id]?.Type.Name;
        }

        public void SetValue(string str)
        {
            if (Guid.TryParse(str, out Guid result))
            {
                SAttribute.SetValue(Parent, result);
                _wrong.enabled = false;
            }
            else
            {
                _wrong.enabled = true;
            }
        }

        public void OpenClassSelect()
        {
            var attr = SAttribute.Info.GetCustomAttribute<TypeLimitAttribute>();
            UnityDataManagerService.OpenClassSelectPanel(attr?.LimitType, SetClass);
            return;

            void SetClass(Type classType)
            {
                SetValue(TypeManager.Instance[classType].ToString());
            }
        }
    }
}