#region

using Secyud.Ugf;
using Secyud.Ugf.BasicComponents;
using UnityEngine.Events;

#endregion

namespace UnityEngine
{
	public static class UgfComponentExtensions
	{
		public static void Translate(this SText[] texts, params string[] origins)
		{
			for (int i = 0; i < origins.Length; i++)
				texts[i].Translate(origins[i]);
		}

		public static void Translate(this SText text, string origin)
		{
			text.text = U.T[origin];
		}

		public static void Set(this SText[] texts, params string[] origins)
		{
			for (int i = 0; i < origins.Length; i++)
				texts[i].Set(origins[i]);
		}

		public static void Set(this SText text, string origin)
		{
			text.text = origin;
		}

		public static void Bind(this SInputField[] controls, params UnityAction<string>[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
				controls[i].Bind(actions[i]);
		}

		public static void Bind(this SSlider[] controls, params UnityAction<float>[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
				controls[i].Bind(actions[i]);
		}

		public static void Bind(this SToggle[] controls, params UnityAction<bool>[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
				controls[i].Bind(actions[i]);
		}

		public static void Bind(this SDropdown[] controls, params UnityAction<int>[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
				controls[i].Bind(actions[i]);
		}

		public static void Bind(this SButton[] controls, params UnityAction[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
				controls[i].Bind(actions[i]);
		}

		public static TComponent Instantiate<TComponent>(
			this IObjectAccessor<TComponent> accessor,
			Transform parent = null)
			where TComponent : Component
		{
			return accessor.Value.Instantiate(parent);
		}
	}
}