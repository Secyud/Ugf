using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    public class TableCell : MonoBehaviour
    {
        public object CellObject { get; protected set; }

        public virtual void SetObject(object cellObject)
        {
            CellObject = cellObject;
            if (cellObject is null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}