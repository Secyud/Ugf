#if UNITY_EDITOR

using Secyud.Ugf.Unity.Ui;
using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.Editor
{
    [CustomEditor(typeof(FaceSlider))]
    public class FaceSliderEditor : SelectableEditor
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

    [CustomEditor(typeof(CircleImage))]
    public class CircleImageEditor : ImageEditor
    {
        private SerializedProperty _triangleNum;
        private SerializedProperty _radius;

        protected override void OnEnable()
        {
            base.OnEnable();
            _triangleNum = serializedObject.FindProperty("_triangleNum");
            _radius = serializedObject.FindProperty("_radius");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_triangleNum);
            EditorGUILayout.PropertyField(_radius);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif