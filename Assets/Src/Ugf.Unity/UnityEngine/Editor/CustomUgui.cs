#region

using Secyud.Ugf.Unity.Components;
using UnityEditor;

#endregion

namespace UnityEngine
{
    [CustomEditor(typeof(SDualToggle))]
    public class CustomDualToggle : Editor
    {
    }

    [CustomEditor(typeof(SStateToggle))]
    public class CustomStateToggle : Editor
    {
    }
}