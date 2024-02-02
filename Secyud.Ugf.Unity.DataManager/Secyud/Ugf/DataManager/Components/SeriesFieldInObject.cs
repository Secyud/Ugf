using System;
using System.Collections;
using System.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    [RequireComponent(typeof(LayoutTrigger))]
    public class SeriesFieldInObject : FieldInObject
    {
        [SerializeField] private Button AddItemButton;
        private List<FieldInSeries> _fields;
        private LayoutTrigger _content;
        private FieldInSeries _prefab;
        public IList List { get; private set; }
        public Type ElementType { get; private set; }

        private void Awake()
        {
            _fields = new List<FieldInSeries>();
            _content = GetComponent<LayoutTrigger>();
        }

        protected override void BindValue(object value)
        {
            foreach (FieldInSeries field in _fields)
            {
                Destroy(field.gameObject);
            }

            _fields.Clear();

            _prefab = UnityDataManagerService
                .GetFieldInSeries(Field.Type);

            List = (IList)GetValue();

            for (int i = 0; i < List.Count; i++)
            {
                FieldInSeries field = _prefab.Instantiate(_content.transform);
                field.Bind(this, i);
                _fields.Add(field);
            }

            AddItemButton.interactable = List.IsFixedSize;

            ElementType = Field.Info.FieldType;
            ElementType = ElementType.HasElementType
                ? ElementType.GetElementType()
                : ElementType.GetGenericArguments()[0];
        }


        public void CreateObject()
        {
            FieldType type = Field.Type & ~ FieldType.List;

            if (type is FieldType.Object)
            {
                UnityDataManagerService.OpenClassSelectPanel(
                    ElementType, CreateFromType);

                void CreateFromType(Type t)
                {
                    Add(TypeManager.Instance[t].CreateInstance());
                }
            }
            else
            {
                Add(type switch
                {
                    FieldType.UInt8   => default(byte), FieldType.UInt16  => default(ushort),
                    FieldType.UInt32  => default(uint), FieldType.UInt64  => default(ulong),
                    FieldType.Int8    => default(sbyte), FieldType.Int16  => default(short),
                    FieldType.Int32   => default(int), FieldType.Int64    => default(long),
                    FieldType.Bool    => default(bool), FieldType.Decimal => default(decimal),
                    FieldType.Single  => default(float), FieldType.Double => default(double),
                    FieldType.String  => default(string), FieldType.Guid  => default(Guid),
                    FieldType.Object  => throw new InvalidOperationException(),
                    FieldType.List    => throw new InvalidOperationException(),
                    FieldType.InValid => throw new InvalidOperationException(),
                    _                 => throw new ArgumentOutOfRangeException()
                });
            }
        }

        public void Add(object obj)
        {
            List.Add(obj);
            FieldInSeries item = _prefab.Instantiate(_content.transform);
            item.Bind(this, List.Count - 1);
            _fields.Add(item);
            RefreshContent(3);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            Destroy(_fields[index].gameObject);

            _fields.RemoveAt(index);

            for (int i = index; i < _fields.Count; i++)
            {
                _fields[i].Index = i;
            }

            RefreshContent(3);
        }

        public void SetVisibility(bool visibility)
        {
            foreach (FieldInSeries item in _fields)
            {
                item.gameObject.SetActive(visibility);
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