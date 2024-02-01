using System;
using Secyud.Ugf.DataManager.Components;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataManagerService : IRegistry
    {
        public static UnityDataManagerService Instance { get; private set; }

        public UnityDataManagerForm Form { get; set; }

        public UnityDataManagerService()
        {
            Instance = this;
        }

        public RectTransform EditContent => Form.UnityDataEditor.Content.RectTransform;

        public static DataField CreateDataField(FieldType fieldType)
        {
            return Instance.Form
                .FieldContainer
                .GetDataField(fieldType)
                .Instantiate(Instance.EditContent);
        }

        public static void RefreshEditContent()
        {
            Instance.Form.UnityDataEditor.Content.Refresh();
        }

        public static void OpenClassSelectPanel(Type baseType, Action<Type> callback)
        {
            Instance.Form.ClassSelectPanel.OpenClassSelectPanel(baseType, callback,true);
        }
    }
}