using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据单元格，继承此类并实现<see cref="SetObject"/>
    /// 以自定义单元格功能。
    /// </summary>
    public class TableCell : MonoBehaviour
    {
        [SerializeField] private bool _setInactiveIfNull = true;
        public object CellObject { get; protected set; }

        public virtual void SetObject(object cellObject)
        {
            CellObject = cellObject;
            if (_setInactiveIfNull)
            {
                gameObject.SetActive(cellObject is not null);
            }
        }
    }
}