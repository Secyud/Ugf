using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class ObjectField : DataField
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

        public override void Bind(object parent, SAttribute sAttribute)
        {
            base.Bind(parent, sAttribute);

            BindObject(sAttribute.GetValue(parent));
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
                DataField field = UnityDataManagerService
                    .CreateDataField(attribute.Type);
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
            UnityDataManagerService.RefreshEditContent();
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
            UnityDataManagerService.OpenClassSelectPanel(
                SAttribute.Info.FieldType, CreateFromType);
            return;

            void CreateFromType(Type t)
            {
                object instance = TypeManager.Instance[t].CreateInstance();
                SAttribute.SetValue(Parent, instance);
                BindObject(instance);
                UnityDataManagerService.RefreshEditContent();
            }
        }

        public void DestroyObject()
        {
            if (Parent is null)
            {
                UgfLogger.LogError("Cannot delete root object!");
                return;
            }
            
            SAttribute.SetValue(Parent, null);
            BindObject(null);
            UnityDataManagerService.RefreshEditContent();
        }

        public void SetThisVisibility(bool visibility)
        {
            SetVisibility(visibility, true);
        }
    }
}