using System;
using System.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Secyud.Ugf.DataManager.Components
{
    public class ObjectFieldInSeries : FieldInSeries
    {
        [SerializeField] protected TextMeshProUGUI ClassNameTip;
        [SerializeField] protected TextMeshProUGUI CreateButton;
        [SerializeField] protected TextMeshProUGUI DeleteButton;
        private LayoutTrigger _content;
        private List<FieldInObject> _fields;

        private void Awake()
        {
            _content = GetComponent<LayoutTrigger>();
            _fields = new List<FieldInObject>();
        }

        protected override void BindValue(object value)
        {
            foreach (FieldInObject field in _fields)
            {
                Destroy(field.gameObject);
            }

            _fields.Clear();

            if (value is null)
            {
                ClassNameTip.text = "Null";
                CreateButton.gameObject.SetActive(true);
                DeleteButton.gameObject.SetActive(false);
                return;
            }

            ClassNameTip.text = value.GetType().Name;

            TypeDescriptor d = TypeManager.Instance[value.GetType()];
            List<SAttribute> attributes = ListPool<SAttribute>.Get();
            d.Properties.FillAttributes(attributes);
            foreach (SAttribute attribute in attributes)
            {
                FieldInObject field = UnityDataManagerService
                    .GetFieldInObject(attribute.Type)
                    .Instantiate(_content.transform);
                field.BindObject(this, attribute);
                _fields.Add(field);
            }

            ListPool<SAttribute>.Release(attributes);
            
            CreateButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(true);
        }


        public void CreateObject()
        {
            UnityDataManagerService.OpenClassSelectPanel(
                SeriesField.ElementType, CreateFromType);
            return;

            void CreateFromType(Type t)
            {
                object instance = TypeManager.Instance[t].CreateInstance();
                SetValue(instance);
                BindValue(instance);
                RefreshContent(3);
            }
        }
        
        public void DestroyObject()
        {
            if (SeriesField.List.IsFixedSize)
            {
                SetValue(null);
                BindValue(null);
                RefreshContent(3);
            }
            else
            {
                RemoveSelf();
            }
        }

        public void SetVisibility(bool visibility)
        {
            foreach (FieldInObject field in _fields)
            {
                field.gameObject.SetActive(visibility);
            }

            RefreshContent(3);
        }

        public override void RefreshContent(int level)
        {
            _content.Refresh(level);
            base.RefreshContent(level);
        }
    }
}