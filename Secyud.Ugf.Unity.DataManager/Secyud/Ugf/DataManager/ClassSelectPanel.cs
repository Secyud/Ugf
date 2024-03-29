﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Abstraction;
using Secyud.Ugf.Unity.TableComponents;
using Secyud.Ugf.Unity.TableComponents.LocalTable;
using Secyud.Ugf.Unity.TableComponents.UiFunctions;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class ClassSelectPanel : MonoBehaviour
    {
        [SerializeField] private Table _table;
        [SerializeField] private FilterInput _filterInput;
        private SingleSelect _singleSelect;
        private Action<Type> _callback;
        private List<TypeDescriptor> _list;



        private void Awake()
        {
            _list = new List<TypeDescriptor>();
            _table.SetLocalSource(() => _list);
            _table.InitLocalFilterInput(_filterInput, new TypeNameFilter());
            _singleSelect = _table.GetOrAddComponent<SingleSelect>();
            _table.Refresh(4);
        }

        public void OpenClassSelectPanel(Type baseType, Action<Type> callBack,bool dependency)
        {
            gameObject.SetActive(true);
            Bind(baseType, callBack,dependency);
        }
        
        public void Bind(Type baseType, Action<Type> callBack,bool dependency)
        {
            _callback = callBack;
            _list.Clear();

            _list.AddRange(TypeManager.Instance
                .GetRegisteredType(baseType,dependency)
                .Select(t => new TypeDescriptor(t.Type)));

            _table.Refresh(3);
        }

        public void OnEnsure()
        {
            if (_singleSelect.SelectedObject is
                TypeDescriptor descriptor)
            {
                _callback.Invoke(descriptor.Type);
            }
        }

        private class TypeNameFilter : ILocalFilterDescriptor, IFilterStringDescriptor
        {
            public bool Filter(object target)
            {
                if (target is not TypeDescriptor typeDescriptor)
                    return false;

                string name = typeDescriptor.Name;
                int j = 0, l = FilterString.Length;
                foreach (char ci in name)
                {
                    if (FilterString[j] % 32 != ci % 32)
                        continue;
                    if (++j == l) return true;
                }

                return false;
            }

            public string FilterString { get; set; }
        }

        private class TypeDescriptor : IHasName, IHasDescription
        {
            public TypeDescriptor(Type type)
            {
                Type = type;
            }

            public Type Type { get; }

            public string Description
            {
                get
                {
                    DescriptionAttribute customAttribute =
                        Type.GetCustomAttribute<DescriptionAttribute>();
                    return customAttribute?.Description;
                }
            }

            public string Name => Type.Name;
        }
    }
}