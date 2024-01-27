using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据运算管理，继承此类实现自定义的筛选排序方式。
    /// </summary>
    public abstract class TableDataOperator : MonoBehaviour
    {
        public Table Table { get; internal set; }

        public abstract void Apply();
    }
}