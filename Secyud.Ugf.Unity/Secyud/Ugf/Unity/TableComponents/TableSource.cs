using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public abstract class TableSource : MonoBehaviour
    {
        public Table Table { get; internal set; }
        public abstract void Apply();
    }
}