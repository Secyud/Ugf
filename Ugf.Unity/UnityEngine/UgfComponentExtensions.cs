using Secyud.Ugf;
using Secyud.Ugf.BasicComponents;
using UnityEngine.Events;

namespace UnityEngine
{
    public static class UgfComponentExtensions
    {
        
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
    }
}