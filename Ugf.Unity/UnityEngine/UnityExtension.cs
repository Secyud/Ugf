#region

using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.Localization;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UnityEngine
{
    public static class UnityExtension
    {
        public static List<TMP_Dropdown.OptionData> GetOptionsFromEnum(
            this IStringLocalizer l, Type enumType, string prefix = null, string suffix = null)
        {
            return Enum
                .GetNames(enumType)
                .Select(u => new TMP_Dropdown.OptionData(l[$"{prefix}{u}{suffix}"]))
                .ToList();
        }

        public static void SetRatio(MonoBehaviour left, MonoBehaviour right, float value)
        {
            if (value is <= 0 or >= 1)
                return;
            var rectLeft = left.GetComponent<RectTransform>();
            rectLeft.anchorMax.Set(value, rectLeft.anchorMax.y);
            var rectRight = right.GetComponent<RectTransform>();
            rectRight.anchorMax.Set(1 - value, rectRight.anchorMax.y);
        }

        public static void CheckBoundary(this RectTransform transform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

            var position = transform.anchoredPosition;

            if (position.x + transform.rect.width > Screen.currentResolution.width)
                position.x = Screen.currentResolution.width - transform.rect.width;

            if (position.y - transform.rect.height < -Screen.currentResolution.height)
                position.y = transform.rect.height - Screen.currentResolution.height;

            transform.anchoredPosition = position;
        }

        public static Vector2 GetMousePosition()
        {
            var x = Input.mousePosition.x / Screen.width * Screen.currentResolution.width;
            var y = (Input.mousePosition.y - Screen.height) / Screen.height * Screen.currentResolution.height;

            return new Vector2(x, y);
        }

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

        public static void Translate(this SText[] texts, params string[] origins)
        {
            for (var i = 0; i < origins.Length; i++)
                texts[i].Translate(origins[i]);
        }

        public static void Translate(this SText text, string origin)
        {
            text.text = Og.L[origin];
        }

        public static void Set(this SText[] texts, params string[] origins)
        {
            for (var i = 0; i < origins.Length; i++)
                texts[i].Set(origins[i]);
        }

        public static void Set(this SText text, string origin)
        {
            text.text = origin;
        }

        public static void Bind(this SInputField[] controls, params UnityAction<string>[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
                controls[i].Bind(actions[i]);
        }

        public static void Bind(this SSlider[] controls, params UnityAction<float>[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
                controls[i].Bind(actions[i]);
        }

        public static void Bind(this SToggle[] controls, params UnityAction<bool>[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
                controls[i].Bind(actions[i]);
        }

        public static void Bind(this SDropdown[] controls, params UnityAction<int>[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
                controls[i].Bind(actions[i]);
        }

        public static void Bind(this SButton[] controls, params UnityAction[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
                controls[i].Bind(actions[i]);
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

        public static string RelativePathTo(this Transform target, Transform root)
        {
            var path = target.name;
            try
            {
                while (target.parent != root)
                {
                    target = target.parent;
                    path = target.name + "/" + path;
                }
            }
            catch
            {
                path = string.Empty;
            }

            return path;
        }
    }
}