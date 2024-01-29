using System.Globalization;
using UnityEngine;

namespace Secyud.Ugf.Unity.EditorComponents
{
    public class StringCallback : MonoBehaviour
    {
        [SerializeField] private EditorCallback<string> _editorCallback;

        public virtual void SetInt32Value(int value)
        {
            _editorCallback.Invoke(value.ToString());
        }

        public virtual void SetSingleValue(float value)
        {
            _editorCallback.Invoke(value.ToString(CultureInfo.InvariantCulture));
        }

        public virtual void SetSingleValueToInt(float value)
        {
            _editorCallback.Invoke(value.ToString("0"));
        }
    }
}