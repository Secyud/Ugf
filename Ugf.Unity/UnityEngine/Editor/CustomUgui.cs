#if UNITY_EDITOR

#region

using Secyud.Ugf.BasicComponents;
using UnityEditor;

#endregion

namespace UnityEngine.Editor
{
    [CustomEditor(typeof(SDualToggle))]
    public class CustomDualToggle : UnityEditor.Editor
    {
    }

    [CustomEditor(typeof(SStateToggle))]
    public class CustomStateToggle : UnityEditor.Editor
    {
    }
}
#endif