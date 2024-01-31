using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class GuidLField : ListItem
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _className;
        [SerializeField] private Image _wrong;

        public override void Bind(ListField parent, int index)
        {
            base.Bind(parent, index);
            Guid id = (Guid)parent.List[index];
            _inputField.SetTextWithoutNotify(id.ToString());
            _inputField.interactable = parent.SAttribute.ShowType == SShowType.Special;
            _className.text = TypeManager.Instance[id]?.Type.Name;
        }

        public void SetValue(string str)
        {
            if (Guid.TryParse(str, out Guid result))
            {
                Parent.List[Index] = result;
                _wrong.enabled = false;
            }
            else
            {
                _wrong.enabled = true;
            }
        }

        public void OpenClassSelect()
        {
            TypeLimitAttribute attr = Parent.SAttribute.Info
                .GetCustomAttribute<TypeLimitAttribute>();
            ClassSelectPanel.OpenClassSelectPanel(attr?.LimitType, SetClass);
            return;

            void SetClass(Type classType)
            {
                SetValue(TypeManager.Instance[classType].ToString());
            }
        }
    }
}