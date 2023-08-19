#if UNITY_EDITOR

#region

using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.TableComponents.FilterComponents;
using UnityEditor;
using UnityEditor.UI;

#endregion

namespace UnityEngine.Editor
{
	[CustomEditor(typeof(SDualToggle))]
	public class SDualToggleEditor : SelectableEditor
	{
		private SerializedProperty _leftClickEvent;
		private SerializedProperty _rightClickEvent;
		private SerializedProperty _graphic;

		protected override void OnEnable()
		{
			base.OnEnable();
			_leftClickEvent = serializedObject.FindProperty("LeftClickEvent");
			_rightClickEvent = serializedObject.FindProperty("RightClickEvent");
			_graphic = serializedObject.FindProperty("Graphic");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_leftClickEvent);
			EditorGUILayout.PropertyField(_rightClickEvent);
			EditorGUILayout.PropertyField(_graphic);
			serializedObject.ApplyModifiedProperties();
		}
	}

	[CustomEditor(typeof(SStateToggle))]
	public class SStateToggleEditor : SelectableEditor
	{
		private SerializedProperty _clickEvent;

		protected override void OnEnable()
		{
			base.OnEnable();
			_clickEvent = serializedObject.FindProperty("ClickEvent");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_clickEvent);
			serializedObject.ApplyModifiedProperties();
		}
	}

	[CustomEditor(typeof(SFaceSlider))]
	public class SFaceSliderEditor : SelectableEditor
	{
		private SerializedProperty _handleRect;
		private SerializedProperty _handleContainerRect;
		private SerializedProperty _sMinValue;
		private SerializedProperty _sMaxValue;
		private SerializedProperty _sValue;
		private SerializedProperty _sOnValueChanged;

		protected override void OnEnable()
		{
			base.OnEnable();
			_handleRect = serializedObject.FindProperty("HandleRect");
			_handleContainerRect = serializedObject.FindProperty("HandleContainerRect");
			_sMinValue = serializedObject.FindProperty("SMinValue");
			_sMaxValue = serializedObject.FindProperty("SMaxValue");
			_sValue = serializedObject.FindProperty("SValue");
			_sOnValueChanged = serializedObject.FindProperty("SOnValueChanged");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_handleRect);
			EditorGUILayout.PropertyField(_handleContainerRect);
			EditorGUILayout.PropertyField(_sMinValue);
			EditorGUILayout.PropertyField(_sMaxValue);
			EditorGUILayout.PropertyField(_sValue);
			EditorGUILayout.PropertyField(_sOnValueChanged);
			serializedObject.ApplyModifiedProperties();
		}
	}

	[CustomEditor(typeof(SImage))]
	public class SImageEditor : ImageEditor
	{
		private SerializedProperty _circle;

		protected override void OnEnable()
		{
			base.OnEnable();
			_circle = serializedObject.FindProperty("Circle");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_circle);
			serializedObject.ApplyModifiedProperties();
		}
	}
	[CustomEditor(typeof(SLabelButton))]
	public class SLabelButtonEditor : ButtonEditor
	{
		private SerializedProperty _label;

		protected override void OnEnable()
		{
			base.OnEnable();
			_label = serializedObject.FindProperty("Label");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_label);
			serializedObject.ApplyModifiedProperties();
		}
	}
	[CustomEditor(typeof(FilterTrigger))]
	public class FilterTriggerEditor : ToggleEditor
	{
		private SerializedProperty _filterTemplate;
		private SerializedProperty _popupTemplate;

		protected override void OnEnable()
		{
			base.OnEnable();
			_filterTemplate = serializedObject.FindProperty("FilterTemplate");
			_popupTemplate = serializedObject.FindProperty("PopupTemplate");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			EditorGUILayout.PropertyField(_filterTemplate);
			EditorGUILayout.PropertyField(_popupTemplate);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif