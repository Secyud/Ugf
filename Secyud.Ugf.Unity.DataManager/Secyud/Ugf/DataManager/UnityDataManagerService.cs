using System;
using Secyud.Ugf.DataManager.Components;
using Secyud.Ugf.DependencyInjection;

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

        public static FieldInObject GetFieldInObject(FieldType fieldType)
        {
            return Instance.Form.FieldContainer.GetFieldInObject(fieldType);
        }

        public static FieldInSeries GetFieldInSeries(FieldType fieldType)
        {
            return Instance.Form.FieldContainer.GetFieldInSeries(fieldType);
        }

        public static void OpenClassSelectPanel(Type baseType, Action<Type> callback)
        {
            Instance.Form.ClassSelectPanel.OpenClassSelectPanel(baseType, callback,true);
        }
    }
}