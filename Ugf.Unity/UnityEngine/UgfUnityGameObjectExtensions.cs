#region

using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.Localization;
using TMPro;
using UnityEngine.Events;

#endregion

namespace UnityEngine
{
    public static class UgfUnityGameObjectExtensions
    {
        public static TComponent Instantiate<TComponent>(this TComponent template, Transform parent)
            where TComponent : Component
        {
            return Object.Instantiate(template, parent);
        }

        public static TComponent InstantiateOnCanvas<TComponent>(this TComponent component)
            where TComponent : Component
        {
            return Object.Instantiate(component, Og.Canvas.transform);
        }

        public static TComponent Instantiate<TComponent>(this TComponent component)
            where TComponent : Component
        {
            return Object.Instantiate(component);
        }

        public static TComponent InstantiateAndAdd<TComponent>(this GameObject ui, Transform parent)
            where TComponent : Component
        {
            var obj = Object.Instantiate(ui, parent);
            return obj.AddComponent<TComponent>();
        }

        public static T GetOrAddComponent<T>(this Transform origin) where T : Component
        {
            if (!origin.TryGetComponent(out T component))
                component = origin.gameObject.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this GameObject origin) where T : Component
        {
            if (!origin.TryGetComponent(out T component))
                component = origin.AddComponent<T>();
            return component;
        }

        public static SButton GetOrAddButton(this GameObject obj, UnityAction action)
        {
            var button = obj.GetOrAddComponent<SButton>();
            button.Bind(action);
            return button;
        }


        public static void Destroy<TComponent>(this TComponent component)
            where TComponent : Component
        {
            if (component)
                Object.Destroy(component.gameObject);
        }

        public static void Replace<TComponent>(this TComponent value, ref TComponent component)
            where TComponent : Component
        {
            if (component == value)
                return;
            component.Destroy();
            component = value;
        }

    }
}