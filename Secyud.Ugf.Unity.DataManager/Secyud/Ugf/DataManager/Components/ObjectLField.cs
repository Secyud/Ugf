using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class ObjectLField : ListItem
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _realType;
        private List<DataField> _subComponents;

        private void Awake()
        {
            _subComponents = new List<DataField>();
        }

        public override Transform Last => _subComponents.Count > 0
            ? _subComponents.Last().Last
            : transform;

        public override void Bind(ListField parent, int index)
        {
            base.Bind(parent, index);

            BindObject(parent.List[index]);
        }

        public void BindObject(object value)
        {
            foreach (DataField subComponent in _subComponents)
            {
                subComponent.Die();
            }

            _subComponents.Clear();

            if (value is null)
            {
                _realType.text = "Null";
                return;
            }
            _realType.text = value.GetType().Name;

            TypeDescriptor d = TypeManager.Instance[value.GetType()];

            List<SAttribute> attributes = ListPool<SAttribute>.Get();

            d.Properties.FillAttributes(attributes);
            foreach (SAttribute attribute in attributes)
            {
                DataField field = FieldContainer.GetDataField(attribute.Type)
                    .Instantiate(UnityDataEditor.Instance.Content.RectTransform);
                field.transform.SetSiblingIndex(Last.GetSiblingIndex() + 1);
                field.Bind(value, attribute);
                _subComponents.Add(field);
            }

            ListPool<SAttribute>.Release(attributes);
        }

        public override void SetVisibility(bool visibility, bool root = false)
        {
            if (!root)
            {
                base.SetVisibility(visibility, false);
                visibility = _toggle.isOn && visibility;
            }

            foreach (DataField field in _subComponents)
            {
                field.SetVisibility(visibility, false);
            }

            UnityDataEditor.Instance.Content.Refresh();
        }

        public override void Die()
        {
            foreach (DataField subComponent in _subComponents)
            {
                subComponent.Die();
            }

            base.Die();
        }

        public void CreateObject()
        {
            Type elementType = Parent.SAttribute.Info.FieldType;
            elementType = elementType.HasElementType
                ? elementType.GetElementType()
                : elementType.GetGenericArguments()[0];


            ClassSelectPanel.OpenClassSelectPanel(elementType, CreateFromType);

            void CreateFromType(Type t)
            {
                object instance = TypeManager.Instance[t].CreateInstance();
                Parent.List[Index] = instance;
                BindObject(instance);
                UnityDataEditor.Instance.Content.Refresh();
            }
        }

        public void DestroyObject()
        {
            Parent.List[Index] = null;
            BindObject(null);
            UnityDataEditor.Instance.Content.Refresh();
        }

        public void SetThisVisibility(bool visibility)
        {
            SetVisibility(visibility, true);
        }
    }
}