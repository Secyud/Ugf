using System.Globalization;

namespace Secyud.Ugf.Unity.EditorComponents
{
    public class StringCallback:EditorCallback<string>
    {
        public virtual void SetInt32Value(int value)
        {
            Invoke(value.ToString());
        }

        public virtual void SetSingleValue(float value)
        {
            Invoke(value.ToString(CultureInfo.CurrentCulture));
        }
    }
}