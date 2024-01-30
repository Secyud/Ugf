using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据运算管理，继承此类实现自定义的筛选排序方式。
    /// </summary>
    public abstract class TableOperator : MonoBehaviour
    {
        public Table Table { get; private set; }
        protected virtual void Awake()
        {
            Table = GetComponent<Table>();
        }
        public abstract void Apply();
    }
}