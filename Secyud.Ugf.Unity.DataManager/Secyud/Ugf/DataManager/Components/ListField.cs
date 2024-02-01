using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class ListField : DataField
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Button _addItem;
        public List<ListItem> SubComponents { get; private set; }
        public IList List { get; private set; }
        public override Transform Last => SubComponents.Count>0?
            SubComponents.Last().Last : transform;

        private ListItem _prefab;


        private void Awake()
        {
            SubComponents = new List<ListItem>();
        }

        public override void Bind(object parent, SAttribute sAttribute)
        {
            base.Bind(parent, sAttribute);
            SubComponents.Clear();
            _prefab = UnityDataManagerService.Instance.Form.FieldContainer
                .GetListItem(sAttribute.Type & ~FieldType.List);
            List = (IList)sAttribute.GetValue(parent);
            int startIndex = transform.GetSiblingIndex();
            for (int i = 0; i < List.Count; i++)
            {
                ListItem item = _prefab.Instantiate(
                    UnityDataManagerService.Instance.EditContent);
                item.transform.SetSiblingIndex(++startIndex);
                item.Bind(this, i);
                SubComponents.Add(item);
            }

            _addItem.interactable = List.IsFixedSize;
        }

        public void CreateObject()
        {
            FieldType type = SAttribute.Type & ~ FieldType.List;

            if (type is FieldType.Object)
            {
                Type elementType = SAttribute.Info.FieldType;
                elementType = elementType.HasElementType
                    ? elementType.GetElementType()
                    : elementType.GetGenericArguments()[0];

                UnityDataManagerService.OpenClassSelectPanel(
                    elementType, CreateFromType);

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
            ListItem item = _prefab.Instantiate(
                UnityDataManagerService.Instance.EditContent);
            item.transform.SetSiblingIndex(Last.GetSiblingIndex() + 1);
            item.Bind(this, List.Count - 1);
            SubComponents.Add(item);
            UnityDataManagerService.RefreshEditContent();
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            SubComponents[index].Die();
            SubComponents.RemoveAt(index);

            for (int i = index; i < SubComponents.Count; i++)
            {
                SubComponents[i].Index = i;
            }

            UnityDataManagerService.RefreshEditContent();
        }

        public override void SetVisibility(bool visibility, bool root = false)
        {
            if (!root)
            {
                base.SetVisibility(visibility, false);
                visibility = _toggle.isOn && visibility;
            }

            foreach (ListItem item in SubComponents)
            {
                item.SetVisibility(visibility, false);
            }

            UnityDataManagerService.RefreshEditContent();
        }

        public void SetThisVisibility(bool visibility)
        {
            SetVisibility(visibility, true);
        }

        public override void Die()
        {
            foreach (ListItem subComponent in SubComponents)
            {
                subComponent.Die();
            }

            base.Die();
        }
    }
}