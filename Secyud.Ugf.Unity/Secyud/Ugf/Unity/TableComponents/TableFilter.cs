using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public abstract class TableFilter : MonoBehaviour
    {
        public Table Table { get; internal set; }

        public abstract void Apply();
    }
}