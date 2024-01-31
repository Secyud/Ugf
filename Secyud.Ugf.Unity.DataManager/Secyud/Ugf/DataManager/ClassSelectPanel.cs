using System;
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


        public static ClassSelectPanel Instance { get; private set; }


        public static void OpenClassSelectPanel(Type baseType, Action<Type> callBack)
        {
            Instance.gameObject.SetActive(true);
            Instance.Bind(baseType, callBack);
        }

        private void Awake()
        {
            if (Instance) Destroy(Instance);
            Instance = this;
            _list = new List<TypeDescriptor>();
            _table.SetLocalSource(() => _list);
            _table.InitLocalFilterInput(_filterInput, new TypeNameFilter());
            _singleSelect = _table.GetOrAddComponent<SingleSelect>();
            _table.Refresh(4);
            gameObject.SetActive(false);
        }


        public void Bind(Type baseType, Action<Type> callBack)
        {
            _callback = callBack;
            _list.Clear();

            _list.AddRange(TypeManager.Instance
                .GetRegisteredType(baseType)
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